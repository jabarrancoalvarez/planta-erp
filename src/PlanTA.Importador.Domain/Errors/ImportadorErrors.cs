using PlanTA.SharedKernel;

namespace PlanTA.Importador.Domain.Errors;

public static class ImportJobErrors
{
    public static Error NotFound(Guid id) =>
        Error.NotFound("ImportJob.NotFound", $"Job de importación '{id}' no encontrado");

    public static Error EstadoInvalido(string estado) =>
        Error.Validation("ImportJob.EstadoInvalido", $"Operación no permitida en estado '{estado}'");
}
