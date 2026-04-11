using PlanTA.Ventas.Application.DTOs;
using PlanTA.SharedKernel.CQRS;

namespace PlanTA.Ventas.Application.Features.Pedidos.GetPedido;

public record GetPedidoQuery(Guid PedidoId) : IQuery<PedidoDetailDto>;
