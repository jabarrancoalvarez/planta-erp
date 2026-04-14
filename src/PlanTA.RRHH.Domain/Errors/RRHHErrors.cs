using PlanTA.SharedKernel;

namespace PlanTA.RRHH.Domain.Errors;

public static class EmpleadoErrors
{
    public static Error NotFound(Guid id) => Error.NotFound("Empleado.NotFound", $"Empleado '{id}' no encontrado");
    public static Error DNIDuplicado(string dni) => Error.Conflict("Empleado.DNIDuplicado", $"Ya existe empleado con DNI '{dni}'");
}

public static class FichajeErrors
{
    public static Error NoJornadaAbierta => Error.Validation("Fichaje.NoJornadaAbierta", "No hay jornada abierta para cerrar");
    public static Error YaFichadoEntrada => Error.Validation("Fichaje.YaFichadoEntrada", "Ya hay una jornada abierta");
    public static Error NotFound(Guid id) => Error.NotFound("Fichaje.NotFound", $"Fichaje '{id}' no encontrado");
}

public static class AusenciaErrors
{
    public static Error NotFound(Guid id) => Error.NotFound("Ausencia.NotFound", $"Ausencia '{id}' no encontrada");
    public static Error FechasInvalidas => Error.Validation("Ausencia.FechasInvalidas", "La fecha de fin debe ser posterior a la de inicio");
}
