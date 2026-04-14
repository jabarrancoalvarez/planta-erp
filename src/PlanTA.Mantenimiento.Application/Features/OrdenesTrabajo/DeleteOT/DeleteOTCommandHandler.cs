using MediatR;
using Microsoft.EntityFrameworkCore;
using PlanTA.Mantenimiento.Application.Interfaces;
using PlanTA.Mantenimiento.Domain.Entities;
using PlanTA.Mantenimiento.Domain.Errors;
using PlanTA.SharedKernel;

namespace PlanTA.Mantenimiento.Application.Features.OrdenesTrabajo.DeleteOT;

public sealed class DeleteOTCommandHandler(
    IMantenimientoDbContext db,
    ICurrentTenant tenant)
    : IRequestHandler<DeleteOTCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(DeleteOTCommand request, CancellationToken cancellationToken)
    {
        var ot = await db.OrdenesTrabajo
            .FirstOrDefaultAsync(
                o => o.Id == new OrdenTrabajoId(request.OrdenTrabajoId) && o.EmpresaId == tenant.EmpresaId,
                cancellationToken);

        if (ot is null)
            return Result<Guid>.Failure(OrdenTrabajoErrors.NotFound(request.OrdenTrabajoId));

        ot.SoftDelete();
        await db.SaveChangesAsync(cancellationToken);

        return Result<Guid>.Success(ot.Id.Value);
    }
}
