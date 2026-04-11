using MediatR;
using Microsoft.EntityFrameworkCore;
using PlanTA.Ventas.Application.DTOs;
using PlanTA.Ventas.Application.Interfaces;
using PlanTA.Ventas.Domain.Entities;
using PlanTA.Ventas.Domain.Errors;
using PlanTA.SharedKernel;

namespace PlanTA.Ventas.Application.Features.Expediciones.GetExpedicion;

public sealed class GetExpedicionQueryHandler(
    IVentasDbContext db,
    ICurrentTenant tenant)
    : IRequestHandler<GetExpedicionQuery, Result<ExpedicionDetailDto>>
{
    public async Task<Result<ExpedicionDetailDto>> Handle(GetExpedicionQuery request, CancellationToken cancellationToken)
    {
        var expedicion = await db.Expediciones
            .AsNoTracking()
            .Include(e => e.Lineas)
            .Where(e => e.Id == new ExpedicionId(request.ExpedicionId) && e.EmpresaId == tenant.EmpresaId)
            .FirstOrDefaultAsync(cancellationToken);

        if (expedicion is null)
            return Result<ExpedicionDetailDto>.Failure(ExpedicionErrors.NotFound(request.ExpedicionId));

        var codigoPedido = await db.Pedidos
            .AsNoTracking()
            .Where(p => p.Id == expedicion.PedidoId)
            .Select(p => p.Codigo)
            .FirstOrDefaultAsync(cancellationToken);

        var dto = new ExpedicionDetailDto(
            expedicion.Id.Value,
            expedicion.PedidoId.Value,
            codigoPedido ?? string.Empty,
            expedicion.FechaExpedicion,
            expedicion.NumeroSeguimiento,
            expedicion.Transportista,
            expedicion.EstadoExpedicion,
            expedicion.Observaciones,
            expedicion.CreatedAt,
            expedicion.Lineas.Select(l => new LineaExpedicionDto(
                l.Id.Value, l.LineaPedidoId.Value, l.ProductoId,
                l.Cantidad, l.LoteOrigenId)).ToList());

        return Result<ExpedicionDetailDto>.Success(dto);
    }
}
