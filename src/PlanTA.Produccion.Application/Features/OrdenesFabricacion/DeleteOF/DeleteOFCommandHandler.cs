using MediatR;
using Microsoft.EntityFrameworkCore;
using PlanTA.Produccion.Application.Interfaces;
using PlanTA.Produccion.Domain.Entities;
using PlanTA.Produccion.Domain.Errors;
using PlanTA.SharedKernel;

namespace PlanTA.Produccion.Application.Features.OrdenesFabricacion.DeleteOF;

public sealed class DeleteOFCommandHandler(
    IProduccionDbContext db,
    ICurrentTenant tenant)
    : IRequestHandler<DeleteOFCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(DeleteOFCommand request, CancellationToken cancellationToken)
    {
        var of = await db.OrdenesFabricacion
            .FirstOrDefaultAsync(
                o => o.Id == new OrdenFabricacionId(request.OrdenFabricacionId) && o.EmpresaId == tenant.EmpresaId,
                cancellationToken);

        if (of is null)
            return Result<Guid>.Failure(OrdenFabricacionErrors.NotFound(request.OrdenFabricacionId));

        of.SoftDelete();
        await db.SaveChangesAsync(cancellationToken);

        return Result<Guid>.Success(of.Id.Value);
    }
}
