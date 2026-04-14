using FluentValidation;

namespace PlanTA.RRHH.Application.Features.Empleados.CreateEmpleado;

public class CreateEmpleadoValidator : AbstractValidator<CreateEmpleadoCommand>
{
    public CreateEmpleadoValidator()
    {
        RuleFor(x => x.Codigo).NotEmpty().MaximumLength(30);
        RuleFor(x => x.Nombre).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Apellidos).NotEmpty().MaximumLength(150);
        RuleFor(x => x.DNI).NotEmpty().MaximumLength(20);
        RuleFor(x => x.Puesto).NotEmpty().MaximumLength(100);
        RuleFor(x => x.CosteHoraEstandar).GreaterThanOrEqualTo(0);
    }
}
