using FluentValidation;

namespace PlanTA.Produccion.Application.Features.OrdenesFabricacion.CreateOF;

public sealed class CreateOFValidator : AbstractValidator<CreateOFCommand>
{
    public CreateOFValidator()
    {
        RuleFor(x => x.CodigoOF).NotEmpty().MaximumLength(50);
        RuleFor(x => x.ProductoId).NotEmpty();
        RuleFor(x => x.ListaMaterialesId).NotEmpty();
        RuleFor(x => x.CantidadPlanificada).GreaterThan(0);
        RuleFor(x => x.UnidadMedida).NotEmpty().MaximumLength(50);
        RuleFor(x => x.Prioridad).GreaterThanOrEqualTo(0);
        RuleFor(x => x.Observaciones).MaximumLength(2000);
    }
}
