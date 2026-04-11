using FluentValidation;

namespace PlanTA.Compras.Application.Features.Recepciones.CreateRecepcion;

public sealed class CreateRecepcionValidator : AbstractValidator<CreateRecepcionCommand>
{
    public CreateRecepcionValidator()
    {
        RuleFor(x => x.OrdenCompraId).NotEmpty();
        RuleFor(x => x.NumeroAlbaran).MaximumLength(100);
        RuleFor(x => x.Observaciones).MaximumLength(2000);

        RuleForEach(x => x.Lineas).ChildRules(linea =>
        {
            linea.RuleFor(l => l.LineaOrdenCompraId).NotEmpty();
            linea.RuleFor(l => l.ProductoId).NotEmpty();
            linea.RuleFor(l => l.CantidadRecibida).GreaterThan(0);
        });
    }
}
