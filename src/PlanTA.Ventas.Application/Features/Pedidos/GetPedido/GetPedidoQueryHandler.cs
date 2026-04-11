using MediatR;
using Microsoft.EntityFrameworkCore;
using PlanTA.Ventas.Application.DTOs;
using PlanTA.Ventas.Application.Interfaces;
using PlanTA.Ventas.Domain.Entities;
using PlanTA.Ventas.Domain.Errors;
using PlanTA.SharedKernel;

namespace PlanTA.Ventas.Application.Features.Pedidos.GetPedido;

public sealed class GetPedidoQueryHandler(
    IVentasDbContext db,
    ICurrentTenant tenant)
    : IRequestHandler<GetPedidoQuery, Result<PedidoDetailDto>>
{
    public async Task<Result<PedidoDetailDto>> Handle(GetPedidoQuery request, CancellationToken cancellationToken)
    {
        var pedido = await db.Pedidos
            .AsNoTracking()
            .Include(p => p.Lineas)
            .Where(p => p.Id == new PedidoId(request.PedidoId) && p.EmpresaId == tenant.EmpresaId)
            .FirstOrDefaultAsync(cancellationToken);

        if (pedido is null)
            return Result<PedidoDetailDto>.Failure(PedidoErrors.NotFound(request.PedidoId));

        var clienteRazonSocial = await db.Clientes
            .AsNoTracking()
            .Where(c => c.Id == pedido.ClienteId)
            .Select(c => c.RazonSocial)
            .FirstOrDefaultAsync(cancellationToken);

        var dto = new PedidoDetailDto(
            pedido.Id.Value,
            pedido.Codigo,
            pedido.ClienteId.Value,
            clienteRazonSocial ?? string.Empty,
            pedido.FechaEmision,
            pedido.FechaEntregaEstimada,
            pedido.EstadoPedido,
            pedido.DireccionEntrega,
            pedido.Observaciones,
            pedido.Total,
            pedido.CreatedAt,
            pedido.Lineas.Select(l => new LineaPedidoDto(
                l.Id.Value, l.ProductoId, l.Descripcion,
                l.Cantidad, l.PrecioUnitario, l.Descuento,
                l.CantidadEnviada, l.Total)).ToList());

        return Result<PedidoDetailDto>.Success(dto);
    }
}
