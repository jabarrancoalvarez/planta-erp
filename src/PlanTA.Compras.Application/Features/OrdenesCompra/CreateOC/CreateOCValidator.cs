using FluentValidation;

namespace PlanTA.Compras.Application.Features.OrdenesCompra.CreateOC;

public sealed class CreateOCValidator : AbstractValidator<CreateOCCommand>
{
    public CreateOCValidator()
    {
        RuleFor(x => x.Codigo).NotEmpty().MaximumLength(50);
        RuleFor(x => x.ProveedorId).NotEmpty();
        RuleFor(x => x.Observaciones).MaximumLength(2000);
    }
}
