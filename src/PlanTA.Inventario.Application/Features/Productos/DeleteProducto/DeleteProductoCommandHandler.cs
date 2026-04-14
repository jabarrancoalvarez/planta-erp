using MediatR;
using Microsoft.EntityFrameworkCore;
using PlanTA.Inventario.Application.Interfaces;
using PlanTA.Inventario.Domain.Entities;
using PlanTA.Inventario.Domain.Errors;
using PlanTA.SharedKernel;

namespace PlanTA.Inventario.Application.Features.Productos.DeleteProducto;

public sealed class DeleteProductoCommandHandler(
    IInventarioDbContext db,
    ICurrentTenant tenant)
    : IRequestHandler<DeleteProductoCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(DeleteProductoCommand request, CancellationToken cancellationToken)
    {
        var producto = await db.Productos
            .FirstOrDefaultAsync(p => p.Id == new ProductoId(request.ProductoId) && p.EmpresaId == tenant.EmpresaId,
                cancellationToken);

        if (producto is null)
            return Result<Guid>.Failure(ProductoErrors.NotFound(request.ProductoId));

        producto.SoftDelete();
        await db.SaveChangesAsync(cancellationToken);
        return Result<Guid>.Success(producto.Id.Value);
    }
}
