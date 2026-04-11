using PlanTA.SharedKernel;

namespace PlanTA.Produccion.Domain.Errors;

public static class ListaMaterialesErrors
{
    public static Error NotFound(Guid id) =>
        Error.NotFound("BOM.NotFound", $"Lista de materiales con ID '{id}' no encontrada");

    public static Error ProductoDuplicado(Guid productoId) =>
        Error.Conflict("BOM.ProductoDuplicado", $"Ya existe una BOM activa para el producto '{productoId}'");

    public static Error LineaComponenteDuplicado(Guid componenteId) =>
        Error.Conflict("BOM.LineaComponenteDuplicado", $"El componente '{componenteId}' ya existe en esta BOM");
}

public static class RutaProduccionErrors
{
    public static Error NotFound(Guid id) =>
        Error.NotFound("Ruta.NotFound", $"Ruta de produccion con ID '{id}' no encontrada");

    public static Error NumeroOperacionDuplicado(int numero) =>
        Error.Conflict("Ruta.NumeroOperacionDuplicado", $"Ya existe una operacion con numero '{numero}' en esta ruta");
}

public static class OrdenFabricacionErrors
{
    public static Error NotFound(Guid id) =>
        Error.NotFound("OF.NotFound", $"Orden de fabricacion con ID '{id}' no encontrada");

    public static Error CodigoDuplicado(string codigo) =>
        Error.Conflict("OF.CodigoDuplicado", $"Ya existe una OF con codigo '{codigo}'");

    public static Error TransicionInvalida(string estadoActual, string estadoDestino) =>
        Error.Validation("OF.TransicionInvalida",
            $"No se puede cambiar de '{estadoActual}' a '{estadoDestino}'");

    public static Error NoEnCurso(Guid id) =>
        Error.Validation("OF.NoEnCurso",
            $"La OF '{id}' debe estar en curso para registrar produccion o consumo");
}
