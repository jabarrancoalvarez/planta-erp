using MediatR;
using Microsoft.EntityFrameworkCore;
using PlanTA.Ventas.Application.DTOs;
using PlanTA.Ventas.Application.Interfaces;
using PlanTA.SharedKernel;

namespace PlanTA.Ventas.Application.Features.Pedidos.ListPedidos;

public sealed class ListPedidosQueryHandler(
    IVentasDbContext db,
    ICurrentTenant tenant)
    : IRequestHandler<ListPedidosQuery, Result<PagedResult<PedidoListDto>>>
{
    public async Task<Result<PagedResult<PedidoListDto>>> Handle(
        ListPedidosQuery request, CancellationToken cancellationToken)
    {
        var query = db.Pedidos
            .AsNoTracking()
            .Where(p => p.EmpresaId == tenant.EmpresaId);

        if (request.Estado.HasValue)
            query = query.Where(p => p.EstadoPedido == request.Estado.Value);

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var search = request.Search.ToLowerInvariant();
            query = query.Where(p => p.Codigo.ToLower().Contains(search));
        }

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderByDescending(p => p.FechaEmision)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(p => new PedidoListDto(
                p.Id.Value,
                p.Codigo,
                p.ClienteId.Value,
                db.Clientes.Where(c => c.Id == p.ClienteId).Select(c => c.RazonSocial).FirstOrDefault() ?? string.Empty,
                p.FechaEmision,
                p.EstadoPedido,
                p.Lineas.Sum(l => l.Cantidad * l.PrecioUnitario * (1 - l.Descuento / 100))))
            .ToListAsync(cancellationToken);

        return Result<PagedResult<PedidoListDto>>.Success(
            new PagedResult<PedidoListDto>(items, totalCount, request.Page, request.PageSize));
    }
}
