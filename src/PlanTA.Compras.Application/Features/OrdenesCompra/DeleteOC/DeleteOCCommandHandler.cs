using MediatR;
using Microsoft.EntityFrameworkCore;
using PlanTA.Compras.Application.Interfaces;
using PlanTA.Compras.Domain.Entities;
using PlanTA.Compras.Domain.Errors;
using PlanTA.SharedKernel;

namespace PlanTA.Compras.Application.Features.OrdenesCompra.DeleteOC;

public sealed class DeleteOCCommandHandler(
    IComprasDbContext db,
    ICurrentTenant tenant)
    : IRequestHandler<DeleteOCCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(DeleteOCCommand request, CancellationToken cancellationToken)
    {
        var oc = await db.OrdenesCompra
            .FirstOrDefaultAsync(
                o => o.Id == new OrdenCompraId(request.OrdenCompraId) && o.EmpresaId == tenant.EmpresaId,
                cancellationToken);

        if (oc is null)
            return Result<Guid>.Failure(OrdenCompraErrors.NotFound(request.OrdenCompraId));

        oc.SoftDelete();
        await db.SaveChangesAsync(cancellationToken);

        return Result<Guid>.Success(oc.Id.Value);
    }
}
