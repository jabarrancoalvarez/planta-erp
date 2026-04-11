using MediatR;
using Microsoft.EntityFrameworkCore;
using PlanTA.Produccion.Application.Interfaces;
using PlanTA.Produccion.Domain.Entities;
using PlanTA.Produccion.Domain.Errors;
using PlanTA.SharedKernel;

namespace PlanTA.Produccion.Application.Features.OrdenesFabricacion.RegistrarProduccion;

public sealed class RegistrarProduccionCommandHandler(
    IProduccionDbContext db,
    ICurrentTenant tenant)
    : IRequestHandler<RegistrarProduccionCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(RegistrarProduccionCommand request, CancellationToken cancellationToken)
    {
        var of = await db.OrdenesFabricacion
            .Include(o => o.PartesProduccion)
            .Where(o => o.Id == new OrdenFabricacionId(request.OrdenFabricacionId) && o.EmpresaId == tenant.EmpresaId)
            .FirstOrDefaultAsync(cancellationToken);

        if (of is null)
            return Result<Guid>.Failure(OrdenFabricacionErrors.NotFound(request.OrdenFabricacionId));

        var result = of.RegistrarProduccion(
            request.UnidadesBuenas,
            request.UnidadesDefectuosas,
            request.Merma,
            request.LoteGeneradoId,
            request.Observaciones);

        if (result.IsFailure)
            return Result<Guid>.Failure(result.Error!);

        await db.SaveChangesAsync(cancellationToken);

        return Result<Guid>.Success(result.Value!.Id.Value);
    }
}
