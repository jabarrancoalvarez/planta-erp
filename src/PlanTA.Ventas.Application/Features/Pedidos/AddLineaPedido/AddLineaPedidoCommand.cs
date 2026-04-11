using PlanTA.SharedKernel.CQRS;

namespace PlanTA.Ventas.Application.Features.Pedidos.AddLineaPedido;

public record AddLineaPedidoCommand(
    Guid PedidoId,
    Guid ProductoId,
    string Descripcion,
    decimal Cantidad,
    decimal PrecioUnitario,
    decimal Descuento = 0) : ICommand<Guid>;
