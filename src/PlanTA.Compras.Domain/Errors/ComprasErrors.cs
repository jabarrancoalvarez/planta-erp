using PlanTA.SharedKernel;

namespace PlanTA.Compras.Domain.Errors;

public static class ProveedorErrors
{
    public static Error NotFound(Guid id) =>
        Error.NotFound("Proveedor.NotFound", $"Proveedor con ID '{id}' no encontrado");

    public static Error NifDuplicado(string nif) =>
        Error.Conflict("Proveedor.NifDuplicado", $"Ya existe un proveedor con NIF '{nif}'");

    public static Error Inactivo(Guid id) =>
        Error.Validation("Proveedor.Inactivo", $"Proveedor '{id}' esta inactivo");
}

public static class OrdenCompraErrors
{
    public static Error NotFound(Guid id) =>
        Error.NotFound("OC.NotFound", $"Orden de compra con ID '{id}' no encontrada");

    public static Error CodigoDuplicado(string codigo) =>
        Error.Conflict("OC.CodigoDuplicado", $"Ya existe una OC con codigo '{codigo}'");

    public static Error TransicionInvalida(string estadoActual, string estadoDestino) =>
        Error.Validation("OC.TransicionInvalida",
            $"No se puede cambiar de '{estadoActual}' a '{estadoDestino}'");

    public static Error NoBorrador(Guid id) =>
        Error.Validation("OC.NoBorrador",
            $"La OC '{id}' debe estar en borrador para agregar lineas");

    public static Error SinLineas(Guid id) =>
        Error.Validation("OC.SinLineas",
            $"La OC '{id}' no tiene lineas y no puede enviarse");
}

public static class RecepcionErrors
{
    public static Error NotFound(Guid id) =>
        Error.NotFound("Recepcion.NotFound", $"Recepcion con ID '{id}' no encontrada");

    public static Error TransicionInvalida(string estadoActual, string estadoDestino) =>
        Error.Validation("Recepcion.TransicionInvalida",
            $"No se puede cambiar de '{estadoActual}' a '{estadoDestino}'");

    public static Error CantidadExcedida(Guid lineaOCId, decimal pendiente, decimal recibida) =>
        Error.Validation("Recepcion.CantidadExcedida",
            $"Linea OC '{lineaOCId}': pendiente {pendiente}, intentando recibir {recibida}");
}
