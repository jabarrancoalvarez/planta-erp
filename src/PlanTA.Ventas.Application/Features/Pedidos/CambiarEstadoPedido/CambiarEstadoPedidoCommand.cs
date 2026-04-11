using PlanTA.Ventas.Domain.Enums;
using PlanTA.SharedKernel.CQRS;

namespace PlanTA.Ventas.Application.Features.Pedidos.CambiarEstadoPedido;

public record CambiarEstadoPedidoCommand(
    Guid PedidoId,
    EstadoPedido EstadoDestino,
    string? Motivo = null) : ICommand<bool>;
