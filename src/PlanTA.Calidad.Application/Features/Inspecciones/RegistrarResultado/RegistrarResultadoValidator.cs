using FluentValidation;

namespace PlanTA.Calidad.Application.Features.Inspecciones.RegistrarResultado;

public sealed class RegistrarResultadoValidator : AbstractValidator<RegistrarResultadoCommand>
{
    public RegistrarResultadoValidator()
    {
        RuleFor(x => x.InspeccionId).NotEmpty();
        RuleFor(x => x.CriterioInspeccionId).NotEmpty();
        RuleFor(x => x.ValorMedido).MaximumLength(500);
        RuleFor(x => x.Observaciones).MaximumLength(2000);
    }
}
