using PlanTA.Ventas.Application.DTOs;
using PlanTA.Ventas.Domain.Enums;
using PlanTA.SharedKernel.CQRS;

namespace PlanTA.Ventas.Application.Features.Pedidos.ListPedidos;

public record ListPedidosQuery(
    string? Search = null,
    EstadoPedido? Estado = null,
    int Page = 1,
    int PageSize = 20) : IQuery<PagedResult<PedidoListDto>>;
