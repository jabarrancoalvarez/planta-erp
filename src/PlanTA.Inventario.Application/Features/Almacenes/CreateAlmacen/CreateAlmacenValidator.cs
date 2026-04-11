using FluentValidation;

namespace PlanTA.Inventario.Application.Features.Almacenes.CreateAlmacen;

public sealed class CreateAlmacenValidator : AbstractValidator<CreateAlmacenCommand>
{
    public CreateAlmacenValidator()
    {
        RuleFor(x => x.Nombre).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Direccion).MaximumLength(500);
    }
}
