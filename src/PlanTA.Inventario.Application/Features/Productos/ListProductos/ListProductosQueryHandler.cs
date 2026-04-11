using MediatR;
using Microsoft.EntityFrameworkCore;
using PlanTA.Inventario.Application.DTOs;
using PlanTA.Inventario.Application.Interfaces;
using PlanTA.SharedKernel;

namespace PlanTA.Inventario.Application.Features.Productos.ListProductos;

public sealed class ListProductosQueryHandler(
    IInventarioDbContext db,
    ICurrentTenant tenant)
    : IRequestHandler<ListProductosQuery, Result<PagedResult<ProductoListDto>>>
{
    public async Task<Result<PagedResult<ProductoListDto>>> Handle(
        ListProductosQuery request, CancellationToken cancellationToken)
    {
        var query = db.Productos
            .AsNoTracking()
            .Where(p => p.EmpresaId == tenant.EmpresaId);

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var search = request.Search.ToLowerInvariant();
            query = query.Where(p =>
                p.Nombre.ToLower().Contains(search) ||
                p.SKU.Value.ToLower().Contains(search));
        }

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderBy(p => p.Nombre)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(p => new ProductoListDto(
                p.Id.Value, p.SKU.Value, p.Nombre,
                p.Tipo, p.UnidadMedida, p.PrecioVenta, p.Activo))
            .ToListAsync(cancellationToken);

        return Result<PagedResult<ProductoListDto>>.Success(
            new PagedResult<ProductoListDto>(items, totalCount, request.Page, request.PageSize));
    }
}
