using PlanTA.SharedKernel.CQRS;

namespace PlanTA.Ventas.Application.Features.Pedidos.UpdatePedido;

public record UpdatePedidoCommand(
    Guid PedidoId,
    DateTimeOffset? FechaEntregaEstimada,
    string? DireccionEntrega,
    string? Observaciones) : ICommand<bool>;
