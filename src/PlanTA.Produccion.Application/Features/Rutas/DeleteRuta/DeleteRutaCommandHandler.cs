using MediatR;
using Microsoft.EntityFrameworkCore;
using PlanTA.Produccion.Application.Interfaces;
using PlanTA.Produccion.Domain.Entities;
using PlanTA.Produccion.Domain.Errors;
using PlanTA.SharedKernel;

namespace PlanTA.Produccion.Application.Features.Rutas.DeleteRuta;

public sealed class DeleteRutaCommandHandler(
    IProduccionDbContext db,
    ICurrentTenant tenant)
    : IRequestHandler<DeleteRutaCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(DeleteRutaCommand request, CancellationToken cancellationToken)
    {
        var ruta = await db.RutasProduccion
            .FirstOrDefaultAsync(
                r => r.Id == new RutaProduccionId(request.RutaProduccionId) && r.EmpresaId == tenant.EmpresaId,
                cancellationToken);

        if (ruta is null)
            return Result<Guid>.Failure(RutaProduccionErrors.NotFound(request.RutaProduccionId));

        ruta.SoftDelete();
        await db.SaveChangesAsync(cancellationToken);

        return Result<Guid>.Success(ruta.Id.Value);
    }
}
