using PlanTA.SharedKernel;

namespace PlanTA.OEE.Domain.Errors;

public static class RegistroOEEErrors
{
    public static Error NotFound(Guid id) => Error.NotFound("RegistroOEE.NotFound", $"Registro '{id}' no encontrado");
    public static Error TiemposInvalidos =>
        Error.Validation("RegistroOEE.TiemposInvalidos", "El tiempo de funcionamiento no puede exceder el tiempo planificado");
    public static Error PiezasInvalidas =>
        Error.Validation("RegistroOEE.PiezasInvalidas", "Las piezas buenas no pueden exceder las piezas totales");
}
