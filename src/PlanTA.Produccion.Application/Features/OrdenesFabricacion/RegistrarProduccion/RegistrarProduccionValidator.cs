using FluentValidation;

namespace PlanTA.Produccion.Application.Features.OrdenesFabricacion.RegistrarProduccion;

public sealed class RegistrarProduccionValidator : AbstractValidator<RegistrarProduccionCommand>
{
    public RegistrarProduccionValidator()
    {
        RuleFor(x => x.OrdenFabricacionId).NotEmpty();
        RuleFor(x => x.UnidadesBuenas).GreaterThanOrEqualTo(0);
        RuleFor(x => x.UnidadesDefectuosas).GreaterThanOrEqualTo(0);
        RuleFor(x => x.Merma).GreaterThanOrEqualTo(0);
        RuleFor(x => x.Observaciones).MaximumLength(2000);
    }
}
