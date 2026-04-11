using PlanTA.SharedKernel.CQRS;

namespace PlanTA.Ventas.Application.Features.Pedidos.CreatePedido;

public record CreatePedidoCommand(
    string Codigo,
    Guid ClienteId,
    DateTimeOffset? FechaEntregaEstimada = null,
    string? DireccionEntrega = null,
    string? Observaciones = null) : ICommand<Guid>;
