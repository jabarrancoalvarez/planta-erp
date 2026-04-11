using FluentValidation;

namespace PlanTA.Produccion.Application.Features.BOM.CreateBOM;

public sealed class CreateBOMValidator : AbstractValidator<CreateBOMCommand>
{
    public CreateBOMValidator()
    {
        RuleFor(x => x.ProductoId).NotEmpty();
        RuleFor(x => x.Nombre).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Descripcion).MaximumLength(1000);
    }
}
