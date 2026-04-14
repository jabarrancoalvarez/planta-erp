using MediatR;
using Microsoft.EntityFrameworkCore;
using PlanTA.Produccion.Application.Interfaces;
using PlanTA.Produccion.Domain.Entities;
using PlanTA.Produccion.Domain.Errors;
using PlanTA.SharedKernel;

namespace PlanTA.Produccion.Application.Features.OrdenesFabricacion.UpdateOF;

public sealed class UpdateOFCommandHandler(
    IProduccionDbContext db,
    ICurrentTenant tenant)
    : IRequestHandler<UpdateOFCommand, Result<bool>>
{
    public async Task<Result<bool>> Handle(UpdateOFCommand request, CancellationToken cancellationToken)
    {
        var of = await db.OrdenesFabricacion
            .Where(o => o.Id == new OrdenFabricacionId(request.OrdenFabricacionId) && o.EmpresaId == tenant.EmpresaId)
            .FirstOrDefaultAsync(cancellationToken);

        if (of is null)
            return Result<bool>.Failure(OrdenFabricacionErrors.NotFound(request.OrdenFabricacionId));

        var result = of.Editar(request.Prioridad, request.Observaciones);
        if (result.IsFailure)
            return result;

        await db.SaveChangesAsync(cancellationToken);
        return Result<bool>.Success(true);
    }
}
