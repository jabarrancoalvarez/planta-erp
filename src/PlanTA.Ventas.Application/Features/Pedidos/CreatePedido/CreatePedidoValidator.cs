using FluentValidation;

namespace PlanTA.Ventas.Application.Features.Pedidos.CreatePedido;

public sealed class CreatePedidoValidator : AbstractValidator<CreatePedidoCommand>
{
    public CreatePedidoValidator()
    {
        RuleFor(x => x.Codigo).NotEmpty().MaximumLength(50);
        RuleFor(x => x.ClienteId).NotEmpty();
        RuleFor(x => x.DireccionEntrega).MaximumLength(500);
        RuleFor(x => x.Observaciones).MaximumLength(2000);
    }
}
