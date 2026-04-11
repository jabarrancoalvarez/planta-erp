using FluentValidation;

namespace PlanTA.Inventario.Application.Features.Productos.CreateProducto;

public sealed class CreateProductoValidator : AbstractValidator<CreateProductoCommand>
{
    public CreateProductoValidator()
    {
        RuleFor(x => x.SKU).NotEmpty().MaximumLength(50);
        RuleFor(x => x.Nombre).NotEmpty().MaximumLength(200);
        RuleFor(x => x.PrecioCompra).GreaterThanOrEqualTo(0);
        RuleFor(x => x.PrecioVenta).GreaterThanOrEqualTo(0);
    }
}
