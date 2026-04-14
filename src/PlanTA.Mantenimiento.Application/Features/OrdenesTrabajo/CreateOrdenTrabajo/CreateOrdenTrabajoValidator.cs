using FluentValidation;

namespace PlanTA.Mantenimiento.Application.Features.OrdenesTrabajo.CreateOrdenTrabajo;

public sealed class CreateOrdenTrabajoValidator : AbstractValidator<CreateOrdenTrabajoCommand>
{
    public CreateOrdenTrabajoValidator()
    {
        RuleFor(x => x.Codigo).NotEmpty().MaximumLength(50);
        RuleFor(x => x.Titulo).NotEmpty().MaximumLength(200);
        RuleFor(x => x.ActivoId).NotEmpty();
        RuleFor(x => x.HorasEstimadas).GreaterThanOrEqualTo(0);
    }
}
