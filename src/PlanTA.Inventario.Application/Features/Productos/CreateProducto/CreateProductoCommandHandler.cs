using MediatR;
using Microsoft.EntityFrameworkCore;
using PlanTA.Inventario.Application.Interfaces;
using PlanTA.Inventario.Domain.Entities;
using PlanTA.Inventario.Domain.Errors;
using PlanTA.SharedKernel;

namespace PlanTA.Inventario.Application.Features.Productos.CreateProducto;

public sealed class CreateProductoCommandHandler(
    IInventarioDbContext db,
    ICurrentTenant tenant)
    : IRequestHandler<CreateProductoCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(CreateProductoCommand request, CancellationToken cancellationToken)
    {
        var skuExists = await db.Productos
            .AnyAsync(p => p.SKU.Value == request.SKU.ToUpperInvariant() && p.EmpresaId == tenant.EmpresaId,
                cancellationToken);

        if (skuExists)
            return Result<Guid>.Failure(ProductoErrors.SkuDuplicado(request.SKU));

        var producto = Producto.Crear(
            request.SKU, request.Nombre, request.Tipo, request.UnidadMedida,
            tenant.EmpresaId, request.Descripcion,
            request.CategoriaId.HasValue ? new CategoriaProductoId(request.CategoriaId.Value) : null,
            request.PrecioCompra, request.PrecioVenta);

        db.Productos.Add(producto);
        await db.SaveChangesAsync(cancellationToken);

        return Result<Guid>.Success(producto.Id.Value);
    }
}
