using FluentValidation;

namespace PlanTA.Calidad.Application.Features.NoConformidades.AddAccionCorrectiva;

public sealed class AddAccionCorrectivaValidator : AbstractValidator<AddAccionCorrectivaCommand>
{
    public AddAccionCorrectivaValidator()
    {
        RuleFor(x => x.NoConformidadId).NotEmpty();
        RuleFor(x => x.Descripcion).NotEmpty().MaximumLength(2000);
    }
}
