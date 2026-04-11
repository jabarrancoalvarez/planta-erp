using PlanTA.SharedKernel;

namespace PlanTA.Ventas.Domain.Errors;

public static class ClienteErrors
{
    public static Error NotFound(Guid id) =>
        Error.NotFound("Cliente.NotFound", $"Cliente con ID '{id}' no encontrado");

    public static Error NifDuplicado(string nif) =>
        Error.Conflict("Cliente.NifDuplicado", $"Ya existe un cliente con NIF '{nif}'");

    public static Error Inactivo(Guid id) =>
        Error.Validation("Cliente.Inactivo", $"Cliente '{id}' esta inactivo");
}

public static class PedidoErrors
{
    public static Error NotFound(Guid id) =>
        Error.NotFound("Pedido.NotFound", $"Pedido con ID '{id}' no encontrado");

    public static Error CodigoDuplicado(string codigo) =>
        Error.Conflict("Pedido.CodigoDuplicado", $"Ya existe un pedido con codigo '{codigo}'");

    public static Error TransicionInvalida(string estadoActual, string estadoDestino) =>
        Error.Validation("Pedido.TransicionInvalida",
            $"No se puede cambiar de '{estadoActual}' a '{estadoDestino}'");

    public static Error NoBorrador(Guid id) =>
        Error.Validation("Pedido.NoBorrador",
            $"El pedido '{id}' debe estar en borrador para agregar lineas");

    public static Error SinLineas(Guid id) =>
        Error.Validation("Pedido.SinLineas",
            $"El pedido '{id}' no tiene lineas y no puede confirmarse");
}

public static class ExpedicionErrors
{
    public static Error NotFound(Guid id) =>
        Error.NotFound("Expedicion.NotFound", $"Expedicion con ID '{id}' no encontrada");

    public static Error TransicionInvalida(string estadoActual, string estadoDestino) =>
        Error.Validation("Expedicion.TransicionInvalida",
            $"No se puede cambiar de '{estadoActual}' a '{estadoDestino}'");

    public static Error CantidadExcedida(Guid lineaPedidoId, decimal pendiente, decimal enviada) =>
        Error.Validation("Expedicion.CantidadExcedida",
            $"Linea pedido '{lineaPedidoId}': pendiente {pendiente}, intentando enviar {enviada}");
}
