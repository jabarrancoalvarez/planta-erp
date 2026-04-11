using FluentValidation;

namespace PlanTA.Calidad.Application.Features.NoConformidades.CreateNC;

public sealed class CreateNCValidator : AbstractValidator<CreateNCCommand>
{
    public CreateNCValidator()
    {
        RuleFor(x => x.Codigo).NotEmpty().MaximumLength(50);
        RuleFor(x => x.OrigenInspeccion).IsInEnum();
        RuleFor(x => x.ReferenciaOrigenId).NotEmpty();
        RuleFor(x => x.Descripcion).NotEmpty().MaximumLength(2000);
        RuleFor(x => x.SeveridadNC).IsInEnum();
    }
}
