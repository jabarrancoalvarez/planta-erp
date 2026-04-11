using FluentValidation;

namespace PlanTA.Ventas.Application.Features.Clientes.CreateCliente;

public sealed class CreateClienteValidator : AbstractValidator<CreateClienteCommand>
{
    public CreateClienteValidator()
    {
        RuleFor(x => x.RazonSocial).NotEmpty().MaximumLength(300);
        RuleFor(x => x.NIF).NotEmpty().MaximumLength(20);
        RuleFor(x => x.Email).NotEmpty().EmailAddress().MaximumLength(200);
        RuleFor(x => x.DireccionEnvio).MaximumLength(500);
        RuleFor(x => x.DireccionFacturacion).MaximumLength(500);
        RuleFor(x => x.Ciudad).MaximumLength(100);
        RuleFor(x => x.CodigoPostal).MaximumLength(10);
        RuleFor(x => x.Pais).MaximumLength(100);
        RuleFor(x => x.Telefono).MaximumLength(50);
        RuleFor(x => x.Web).MaximumLength(300);
    }
}
