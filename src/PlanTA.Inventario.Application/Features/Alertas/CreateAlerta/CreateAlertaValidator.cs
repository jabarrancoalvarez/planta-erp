using FluentValidation;

namespace PlanTA.Inventario.Application.Features.Alertas.CreateAlerta;

public sealed class CreateAlertaValidator : AbstractValidator<CreateAlertaCommand>
{
    public CreateAlertaValidator()
    {
        RuleFor(x => x.ProductoId).NotEmpty();
        RuleFor(x => x.StockMinimo).GreaterThanOrEqualTo(0);
        RuleFor(x => x.StockMaximo).GreaterThan(0);
        RuleFor(x => x.StockMaximo).GreaterThan(x => x.StockMinimo)
            .WithMessage("Stock máximo debe ser mayor que stock mínimo");
    }
}
