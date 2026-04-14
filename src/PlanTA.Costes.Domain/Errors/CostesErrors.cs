using PlanTA.SharedKernel;

namespace PlanTA.Costes.Domain.Errors;

public static class ImputacionCosteErrors
{
    public static Error NotFound(Guid id) =>
        Error.NotFound("ImputacionCoste.NotFound", $"Imputación '{id}' no encontrada");

    public static Error ImporteInvalido =>
        Error.Validation("ImputacionCoste.ImporteInvalido", "El importe debe ser mayor que cero");
}
