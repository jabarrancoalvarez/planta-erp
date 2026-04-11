using MediatR;
using Microsoft.EntityFrameworkCore;
using PlanTA.Ventas.Application.DTOs;
using PlanTA.Ventas.Application.Interfaces;
using PlanTA.Ventas.Domain.Entities;
using PlanTA.SharedKernel;

namespace PlanTA.Ventas.Application.Features.Expediciones.ListExpediciones;

public sealed class ListExpedicionesQueryHandler(
    IVentasDbContext db,
    ICurrentTenant tenant)
    : IRequestHandler<ListExpedicionesQuery, Result<PagedResult<ExpedicionListDto>>>
{
    public async Task<Result<PagedResult<ExpedicionListDto>>> Handle(
        ListExpedicionesQuery request, CancellationToken cancellationToken)
    {
        var query = db.Expediciones
            .AsNoTracking()
            .Where(e => e.EmpresaId == tenant.EmpresaId);

        if (request.PedidoId.HasValue)
            query = query.Where(e => e.PedidoId == new PedidoId(request.PedidoId.Value));

        if (request.Estado.HasValue)
            query = query.Where(e => e.EstadoExpedicion == request.Estado.Value);

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderByDescending(e => e.FechaExpedicion)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(e => new ExpedicionListDto(
                e.Id.Value,
                e.PedidoId.Value,
                db.Pedidos.Where(p => p.Id == e.PedidoId).Select(p => p.Codigo).FirstOrDefault() ?? string.Empty,
                e.FechaExpedicion,
                e.NumeroSeguimiento,
                e.Transportista,
                e.EstadoExpedicion))
            .ToListAsync(cancellationToken);

        return Result<PagedResult<ExpedicionListDto>>.Success(
            new PagedResult<ExpedicionListDto>(items, totalCount, request.Page, request.PageSize));
    }
}
