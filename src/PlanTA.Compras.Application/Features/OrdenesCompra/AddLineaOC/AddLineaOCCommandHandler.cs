using MediatR;
using Microsoft.EntityFrameworkCore;
using PlanTA.Compras.Application.Interfaces;
using PlanTA.Compras.Domain.Entities;
using PlanTA.Compras.Domain.Errors;
using PlanTA.SharedKernel;

namespace PlanTA.Compras.Application.Features.OrdenesCompra.AddLineaOC;

public sealed class AddLineaOCCommandHandler(
    IComprasDbContext db,
    ICurrentTenant tenant)
    : IRequestHandler<AddLineaOCCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(AddLineaOCCommand request, CancellationToken cancellationToken)
    {
        var oc = await db.OrdenesCompra
            .Include(o => o.Lineas)
            .Where(o => o.Id == new OrdenCompraId(request.OrdenCompraId) && o.EmpresaId == tenant.EmpresaId)
            .FirstOrDefaultAsync(cancellationToken);

        if (oc is null)
            return Result<Guid>.Failure(OrdenCompraErrors.NotFound(request.OrdenCompraId));

        var result = oc.AgregarLinea(request.ProductoId, request.Descripcion, request.Cantidad, request.PrecioUnitario);

        if (result.IsFailure)
            return Result<Guid>.Failure(result.Error!);

        await db.SaveChangesAsync(cancellationToken);

        return Result<Guid>.Success(result.Value!.Id.Value);
    }
}
