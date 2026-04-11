using FluentValidation;

namespace PlanTA.Calidad.Application.Features.Plantillas.AddCriterio;

public sealed class AddCriterioValidator : AbstractValidator<AddCriterioCommand>
{
    public AddCriterioValidator()
    {
        RuleFor(x => x.PlantillaId).NotEmpty();
        RuleFor(x => x.Nombre).NotEmpty().MaximumLength(300);
        RuleFor(x => x.TipoMedida).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Descripcion).MaximumLength(1000);
        RuleFor(x => x.UnidadMedida).MaximumLength(50);
        RuleFor(x => x.ValorMaximo)
            .GreaterThanOrEqualTo(x => x.ValorMinimo)
            .When(x => x.ValorMinimo.HasValue && x.ValorMaximo.HasValue);
    }
}
