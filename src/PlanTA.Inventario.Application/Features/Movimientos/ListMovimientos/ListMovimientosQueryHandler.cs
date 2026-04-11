using MediatR;
using Microsoft.EntityFrameworkCore;
using PlanTA.Inventario.Application.DTOs;
using PlanTA.Inventario.Application.Features.Productos.ListProductos;
using PlanTA.Inventario.Application.Interfaces;
using PlanTA.Inventario.Domain.Entities;
using PlanTA.SharedKernel;

namespace PlanTA.Inventario.Application.Features.Movimientos.ListMovimientos;

public sealed class ListMovimientosQueryHandler(
    IInventarioDbContext db,
    ICurrentTenant tenant)
    : IRequestHandler<ListMovimientosQuery, Result<PagedResult<MovimientoStockDto>>>
{
    public async Task<Result<PagedResult<MovimientoStockDto>>> Handle(
        ListMovimientosQuery request, CancellationToken cancellationToken)
    {
        var query = db.Movimientos
            .AsNoTracking()
            .Where(m => m.EmpresaId == tenant.EmpresaId);

        if (request.ProductoId.HasValue)
            query = query.Where(m => m.ProductoId == new ProductoId(request.ProductoId.Value));

        if (request.AlmacenId.HasValue)
            query = query.Where(m => m.AlmacenId == new AlmacenId(request.AlmacenId.Value));

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderByDescending(m => m.CreatedAt)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(m => new MovimientoStockDto(
                m.Id.Value, m.ProductoId.Value, m.AlmacenId.Value,
                m.UbicacionId != null ? m.UbicacionId.Value : null,
                m.LoteId != null ? m.LoteId.Value : null,
                m.Tipo, m.Cantidad, m.CantidadAnterior, m.CantidadPosterior,
                m.Referencia, m.Notas, m.CreatedAt))
            .ToListAsync(cancellationToken);

        return Result<PagedResult<MovimientoStockDto>>.Success(
            new PagedResult<MovimientoStockDto>(items, totalCount, request.Page, request.PageSize));
    }
}
