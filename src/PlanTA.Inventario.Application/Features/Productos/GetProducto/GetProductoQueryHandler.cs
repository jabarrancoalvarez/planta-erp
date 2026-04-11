using MediatR;
using Microsoft.EntityFrameworkCore;
using PlanTA.Inventario.Application.DTOs;
using PlanTA.Inventario.Application.Interfaces;
using PlanTA.Inventario.Domain.Entities;
using PlanTA.Inventario.Domain.Errors;
using PlanTA.SharedKernel;

namespace PlanTA.Inventario.Application.Features.Productos.GetProducto;

public sealed class GetProductoQueryHandler(
    IInventarioDbContext db,
    ICurrentTenant tenant)
    : IRequestHandler<GetProductoQuery, Result<ProductoDto>>
{
    public async Task<Result<ProductoDto>> Handle(GetProductoQuery request, CancellationToken cancellationToken)
    {
        var producto = await db.Productos
            .AsNoTracking()
            .Where(p => p.Id == new ProductoId(request.ProductoId) && p.EmpresaId == tenant.EmpresaId)
            .Select(p => new ProductoDto(
                p.Id.Value, p.SKU.Value, p.Nombre, p.Descripcion,
                p.Tipo, p.UnidadMedida, p.CategoriaId != null ? p.CategoriaId.Value : null,
                p.PrecioCompra, p.PrecioVenta, p.PesoKg,
                p.CodigoBarras, p.ImagenUrl, p.Activo, p.CreatedAt))
            .FirstOrDefaultAsync(cancellationToken);

        return producto is not null
            ? Result<ProductoDto>.Success(producto)
            : Result<ProductoDto>.Failure(ProductoErrors.NotFound(request.ProductoId));
    }
}
