using FluentValidation;

namespace PlanTA.Incidencias.Application.Features.Incidencias.CreateIncidencia;

public sealed class CreateIncidenciaValidator : AbstractValidator<CreateIncidenciaCommand>
{
    public CreateIncidenciaValidator()
    {
        RuleFor(x => x.Codigo).NotEmpty().MaximumLength(50);
        RuleFor(x => x.Titulo).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Descripcion).NotEmpty().MaximumLength(4000);
        RuleFor(x => x.ReportadoPorUserId).NotEmpty();
    }
}
