using PlanTA.SharedKernel;

namespace PlanTA.Facturacion.Domain.Errors;

public static class FacturaErrors
{
    public static Error NotFound(Guid id) => Error.NotFound("Factura.NotFound", $"Factura '{id}' no encontrada");
    public static Error EstadoInvalido(string estado) =>
        Error.Validation("Factura.EstadoInvalido", $"Operación no permitida en estado '{estado}'");
    public static Error SinLineas => Error.Validation("Factura.SinLineas", "La factura no tiene líneas");
}
