using FluentValidation;

namespace PlanTA.Produccion.Application.Features.BOM.AddLineaBOM;

public sealed class AddLineaBOMValidator : AbstractValidator<AddLineaBOMCommand>
{
    public AddLineaBOMValidator()
    {
        RuleFor(x => x.ListaMaterialesId).NotEmpty();
        RuleFor(x => x.ComponenteProductoId).NotEmpty();
        RuleFor(x => x.Cantidad).GreaterThan(0);
        RuleFor(x => x.UnidadMedida).NotEmpty().MaximumLength(50);
        RuleFor(x => x.Merma).GreaterThanOrEqualTo(0).LessThanOrEqualTo(100);
    }
}
