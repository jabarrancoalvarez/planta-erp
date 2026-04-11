using FluentValidation;

namespace PlanTA.Inventario.Application.Features.Lotes.CreateLote;

public sealed class CreateLoteValidator : AbstractValidator<CreateLoteCommand>
{
    public CreateLoteValidator()
    {
        RuleFor(x => x.ProductoId).NotEmpty();
        RuleFor(x => x.Cantidad).GreaterThan(0);
        RuleFor(x => x.CodigoLote).MaximumLength(50).When(x => x.CodigoLote is not null);
    }
}
