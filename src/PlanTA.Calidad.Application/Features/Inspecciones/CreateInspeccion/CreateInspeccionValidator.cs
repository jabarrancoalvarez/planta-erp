using FluentValidation;

namespace PlanTA.Calidad.Application.Features.Inspecciones.CreateInspeccion;

public sealed class CreateInspeccionValidator : AbstractValidator<CreateInspeccionCommand>
{
    public CreateInspeccionValidator()
    {
        RuleFor(x => x.PlantillaInspeccionId).NotEmpty();
        RuleFor(x => x.OrigenInspeccion).IsInEnum();
        RuleFor(x => x.ReferenciaOrigenId).NotEmpty();
        RuleFor(x => x.Observaciones).MaximumLength(2000);
    }
}
