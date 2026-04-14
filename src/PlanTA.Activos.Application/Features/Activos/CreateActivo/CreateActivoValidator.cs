using FluentValidation;

namespace PlanTA.Activos.Application.Features.Activos.CreateActivo;

public sealed class CreateActivoValidator : AbstractValidator<CreateActivoCommand>
{
    public CreateActivoValidator()
    {
        RuleFor(x => x.Codigo).NotEmpty().MaximumLength(50);
        RuleFor(x => x.Nombre).NotEmpty().MaximumLength(200);
        RuleFor(x => x.CosteAdquisicion).GreaterThanOrEqualTo(0);
    }
}
