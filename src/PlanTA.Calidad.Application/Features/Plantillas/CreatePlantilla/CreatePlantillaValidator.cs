using FluentValidation;

namespace PlanTA.Calidad.Application.Features.Plantillas.CreatePlantilla;

public sealed class CreatePlantillaValidator : AbstractValidator<CreatePlantillaCommand>
{
    public CreatePlantillaValidator()
    {
        RuleFor(x => x.Nombre).NotEmpty().MaximumLength(300);
        RuleFor(x => x.OrigenInspeccion).IsInEnum();
        RuleFor(x => x.Descripcion).MaximumLength(1000);
    }
}
