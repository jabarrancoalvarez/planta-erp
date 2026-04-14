using PlanTA.SharedKernel;

namespace PlanTA.Incidencias.Domain.Errors;

public static class IncidenciaErrors
{
    public static Error NotFound(Guid id) =>
        Error.NotFound("Incidencia.NotFound", $"Incidencia '{id}' no encontrada");

    public static Error YaCerrada(Guid id) =>
        Error.Validation("Incidencia.YaCerrada", $"Incidencia '{id}' ya está cerrada");
}
