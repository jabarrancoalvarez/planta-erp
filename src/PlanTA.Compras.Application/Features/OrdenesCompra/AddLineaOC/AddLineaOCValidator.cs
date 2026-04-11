using FluentValidation;

namespace PlanTA.Compras.Application.Features.OrdenesCompra.AddLineaOC;

public sealed class AddLineaOCValidator : AbstractValidator<AddLineaOCCommand>
{
    public AddLineaOCValidator()
    {
        RuleFor(x => x.OrdenCompraId).NotEmpty();
        RuleFor(x => x.ProductoId).NotEmpty();
        RuleFor(x => x.Descripcion).NotEmpty().MaximumLength(500);
        RuleFor(x => x.Cantidad).GreaterThan(0);
        RuleFor(x => x.PrecioUnitario).GreaterThanOrEqualTo(0);
    }
}
