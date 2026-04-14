using MediatR;
using Microsoft.EntityFrameworkCore;
using PlanTA.Produccion.Application.Interfaces;
using PlanTA.Produccion.Domain.Entities;
using PlanTA.Produccion.Domain.Errors;
using PlanTA.SharedKernel;
using PlanTA.SharedKernel.CQRS;

namespace PlanTA.Produccion.Application.Features.Rutas.UpdateRuta;

public record UpdateRutaCommand(Guid RutaId, string Nombre, string? Descripcion) : ICommand<Guid>;

public sealed class UpdateRutaCommandHandler(
    IProduccionDbContext db,
    ICurrentTenant tenant)
    : IRequestHandler<UpdateRutaCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(UpdateRutaCommand request, CancellationToken cancellationToken)
    {
        var ruta = await db.RutasProduccion
            .FirstOrDefaultAsync(
                r => r.Id == new RutaProduccionId(request.RutaId) && r.EmpresaId == tenant.EmpresaId,
                cancellationToken);

        if (ruta is null)
            return Result<Guid>.Failure(RutaProduccionErrors.NotFound(request.RutaId));

        ruta.Actualizar(request.Nombre, request.Descripcion);
        await db.SaveChangesAsync(cancellationToken);
        return Result<Guid>.Success(ruta.Id.Value);
    }
}
