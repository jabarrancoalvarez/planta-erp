using FluentValidation;

namespace PlanTA.Ventas.Application.Features.Expediciones.CreateExpedicion;

public sealed class CreateExpedicionValidator : AbstractValidator<CreateExpedicionCommand>
{
    public CreateExpedicionValidator()
    {
        RuleFor(x => x.PedidoId).NotEmpty();
        RuleFor(x => x.NumeroSeguimiento).MaximumLength(100);
        RuleFor(x => x.Transportista).MaximumLength(200);
        RuleFor(x => x.Observaciones).MaximumLength(2000);

        RuleForEach(x => x.Lineas).ChildRules(linea =>
        {
            linea.RuleFor(l => l.LineaPedidoId).NotEmpty();
            linea.RuleFor(l => l.ProductoId).NotEmpty();
            linea.RuleFor(l => l.Cantidad).GreaterThan(0);
        });
    }
}
