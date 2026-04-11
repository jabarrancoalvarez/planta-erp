using MediatR;
using Microsoft.EntityFrameworkCore;
using PlanTA.Inventario.Application.DTOs;
using PlanTA.Inventario.Application.Features.Productos.ListProductos;
using PlanTA.Inventario.Application.Interfaces;
using PlanTA.Inventario.Domain.Entities;
using PlanTA.SharedKernel;

namespace PlanTA.Inventario.Application.Features.Lotes.ListLotes;

public sealed class ListLotesQueryHandler(
    IInventarioDbContext db,
    ICurrentTenant tenant)
    : IRequestHandler<ListLotesQuery, Result<PagedResult<LoteListDto>>>
{
    public async Task<Result<PagedResult<LoteListDto>>> Handle(
        ListLotesQuery request, CancellationToken cancellationToken)
    {
        var query = db.Lotes
            .AsNoTracking()
            .Where(l => l.EmpresaId == tenant.EmpresaId);

        if (request.ProductoId.HasValue)
            query = query.Where(l => l.ProductoId == new ProductoId(request.ProductoId.Value));

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderByDescending(l => l.FechaRecepcion)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(l => new LoteListDto(
                l.Id.Value, l.Codigo.Value, l.CantidadActual,
                l.Estado, l.FechaCaducidad))
            .ToListAsync(cancellationToken);

        return Result<PagedResult<LoteListDto>>.Success(
            new PagedResult<LoteListDto>(items, totalCount, request.Page, request.PageSize));
    }
}
