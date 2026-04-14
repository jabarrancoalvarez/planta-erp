using PlanTA.SharedKernel;

namespace PlanTA.Activos.Domain.Errors;

public static class ActivoErrors
{
    public static Error NotFound(Guid id) =>
        Error.NotFound("Activo.NotFound", $"Activo con ID '{id}' no encontrado");

    public static Error CodigoDuplicado(string codigo) =>
        Error.Conflict("Activo.CodigoDuplicado", $"Ya existe un activo con código '{codigo}'");

    public static Error PadreInvalido =>
        Error.Validation("Activo.PadreInvalido", "El activo padre no existe o pertenece a otra empresa");
}

public static class DocumentoActivoErrors
{
    public static Error NotFound(Guid id) =>
        Error.NotFound("DocumentoActivo.NotFound", $"Documento con ID '{id}' no encontrado");
}
