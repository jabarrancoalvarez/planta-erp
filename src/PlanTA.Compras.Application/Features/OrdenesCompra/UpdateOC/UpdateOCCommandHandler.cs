using MediatR;
using Microsoft.EntityFrameworkCore;
using PlanTA.Compras.Application.Interfaces;
using PlanTA.Compras.Domain.Entities;
using PlanTA.Compras.Domain.Errors;
using PlanTA.SharedKernel;

namespace PlanTA.Compras.Application.Features.OrdenesCompra.UpdateOC;

public sealed class UpdateOCCommandHandler(
    IComprasDbContext db,
    ICurrentTenant tenant)
    : IRequestHandler<UpdateOCCommand, Result<bool>>
{
    public async Task<Result<bool>> Handle(UpdateOCCommand request, CancellationToken cancellationToken)
    {
        var oc = await db.OrdenesCompra
            .Where(o => o.Id == new OrdenCompraId(request.OrdenCompraId) && o.EmpresaId == tenant.EmpresaId)
            .FirstOrDefaultAsync(cancellationToken);

        if (oc is null)
            return Result<bool>.Failure(OrdenCompraErrors.NotFound(request.OrdenCompraId));

        var result = oc.Editar(request.FechaEntregaEstimada, request.Observaciones);
        if (result.IsFailure)
            return result;

        await db.SaveChangesAsync(cancellationToken);
        return Result<bool>.Success(true);
    }
}
