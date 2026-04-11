using PlanTA.SharedKernel;

namespace PlanTA.Calidad.Domain.Errors;

public static class PlantillaErrors
{
    public static Error NotFound(Guid id) =>
        Error.NotFound("Plantilla.NotFound", $"Plantilla de inspeccion con ID '{id}' no encontrada");

    public static Error Inactiva(Guid id) =>
        Error.Validation("Plantilla.Inactiva", $"La plantilla '{id}' esta inactiva");

    public static Error CriterioNotFound(Guid id) =>
        Error.NotFound("Plantilla.CriterioNotFound", $"Criterio con ID '{id}' no encontrado");
}

public static class InspeccionErrors
{
    public static Error NotFound(Guid id) =>
        Error.NotFound("Inspeccion.NotFound", $"Inspeccion con ID '{id}' no encontrada");

    public static Error YaCompletada(Guid id) =>
        Error.Validation("Inspeccion.YaCompletada", $"La inspeccion '{id}' ya esta completada");

    public static Error ResultadoCriterioNotFound(Guid criterioId) =>
        Error.NotFound("Inspeccion.ResultadoCriterioNotFound",
            $"No se encontro resultado para el criterio '{criterioId}'");

    public static Error ResultadosPendientes(Guid id) =>
        Error.Validation("Inspeccion.ResultadosPendientes",
            $"La inspeccion '{id}' tiene criterios obligatorios sin resultado");
}

public static class NoConformidadErrors
{
    public static Error NotFound(Guid id) =>
        Error.NotFound("NoConformidad.NotFound", $"No conformidad con ID '{id}' no encontrada");

    public static Error TransicionInvalida(string estadoActual, string estadoDestino) =>
        Error.Validation("NoConformidad.TransicionInvalida",
            $"No se puede cambiar de '{estadoActual}' a '{estadoDestino}'");

    public static Error CodigoDuplicado(string codigo) =>
        Error.Conflict("NoConformidad.CodigoDuplicado",
            $"Ya existe una no conformidad con codigo '{codigo}'");
}
