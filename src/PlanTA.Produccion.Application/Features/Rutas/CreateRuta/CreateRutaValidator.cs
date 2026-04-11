using FluentValidation;

namespace PlanTA.Produccion.Application.Features.Rutas.CreateRuta;

public sealed class CreateRutaValidator : AbstractValidator<CreateRutaCommand>
{
    public CreateRutaValidator()
    {
        RuleFor(x => x.ProductoId).NotEmpty();
        RuleFor(x => x.Nombre).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Descripcion).MaximumLength(1000);

        RuleForEach(x => x.Operaciones).ChildRules(op =>
        {
            op.RuleFor(o => o.Numero).GreaterThan(0);
            op.RuleFor(o => o.Nombre).NotEmpty().MaximumLength(200);
            op.RuleFor(o => o.TiempoEstimadoMinutos).GreaterThanOrEqualTo(0);
            op.RuleFor(o => o.CentroTrabajo).NotEmpty().MaximumLength(200);
            op.RuleFor(o => o.Instrucciones).MaximumLength(2000);
        });
    }
}
