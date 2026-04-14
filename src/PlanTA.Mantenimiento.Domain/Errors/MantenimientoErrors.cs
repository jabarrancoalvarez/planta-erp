using PlanTA.SharedKernel;

namespace PlanTA.Mantenimiento.Domain.Errors;

public static class OrdenTrabajoErrors
{
    public static Error NotFound(Guid id) =>
        Error.NotFound("OT.NotFound", $"Orden de trabajo '{id}' no encontrada");

    public static Error EstadoInvalido(string motivo) =>
        Error.Validation("OT.EstadoInvalido", motivo);

    public static Error YaCompletada(Guid id) =>
        Error.Validation("OT.YaCompletada", $"La OT '{id}' ya está completada");
}

public static class PlanMantenimientoErrors
{
    public static Error NotFound(Guid id) =>
        Error.NotFound("Plan.NotFound", $"Plan de mantenimiento '{id}' no encontrado");

    public static Error CodigoDuplicado(string codigo) =>
        Error.Conflict("Plan.CodigoDuplicado", $"Ya existe un plan con código '{codigo}'");
}
