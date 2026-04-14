using PlanTA.SharedKernel.CQRS;

namespace PlanTA.Ventas.Application.Features.Pedidos.DeletePedido;

public record DeletePedidoCommand(Guid PedidoId) : ICommand<Guid>;
