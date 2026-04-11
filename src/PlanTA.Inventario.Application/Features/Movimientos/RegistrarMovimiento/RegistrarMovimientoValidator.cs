using FluentValidation;

namespace PlanTA.Inventario.Application.Features.Movimientos.RegistrarMovimiento;

public sealed class RegistrarMovimientoValidator : AbstractValidator<RegistrarMovimientoCommand>
{
    public RegistrarMovimientoValidator()
    {
        RuleFor(x => x.ProductoId).NotEmpty();
        RuleFor(x => x.AlmacenId).NotEmpty();
        RuleFor(x => x.Cantidad).GreaterThan(0);
        RuleFor(x => x.Tipo).IsInEnum();
    }
}
