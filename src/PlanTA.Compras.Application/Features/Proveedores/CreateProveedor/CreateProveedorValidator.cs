using FluentValidation;

namespace PlanTA.Compras.Application.Features.Proveedores.CreateProveedor;

public sealed class CreateProveedorValidator : AbstractValidator<CreateProveedorCommand>
{
    public CreateProveedorValidator()
    {
        RuleFor(x => x.RazonSocial).NotEmpty().MaximumLength(300);
        RuleFor(x => x.NIF).NotEmpty().MaximumLength(20);
        RuleFor(x => x.Email).NotEmpty().EmailAddress().MaximumLength(200);
        RuleFor(x => x.DiasVencimiento).GreaterThanOrEqualTo(0);
        RuleFor(x => x.DescuentoProntoPago).InclusiveBetween(0, 100);
        RuleFor(x => x.MetodoPago).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Direccion).MaximumLength(500);
        RuleFor(x => x.Ciudad).MaximumLength(100);
        RuleFor(x => x.CodigoPostal).MaximumLength(10);
        RuleFor(x => x.Pais).MaximumLength(100);
        RuleFor(x => x.Telefono).MaximumLength(50);
        RuleFor(x => x.Web).MaximumLength(300);
    }
}
