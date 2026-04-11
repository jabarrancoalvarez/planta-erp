using FluentValidation;

namespace PlanTA.Ventas.Application.Features.Pedidos.AddLineaPedido;

public sealed class AddLineaPedidoValidator : AbstractValidator<AddLineaPedidoCommand>
{
    public AddLineaPedidoValidator()
    {
        RuleFor(x => x.PedidoId).NotEmpty();
        RuleFor(x => x.ProductoId).NotEmpty();
        RuleFor(x => x.Descripcion).NotEmpty().MaximumLength(500);
        RuleFor(x => x.Cantidad).GreaterThan(0);
        RuleFor(x => x.PrecioUnitario).GreaterThanOrEqualTo(0);
        RuleFor(x => x.Descuento).InclusiveBetween(0, 100);
    }
}
