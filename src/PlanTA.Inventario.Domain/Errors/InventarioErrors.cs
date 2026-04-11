using PlanTA.SharedKernel;

namespace PlanTA.Inventario.Domain.Errors;

public static class ProductoErrors
{
    public static Error NotFound(Guid id) =>
        Error.NotFound("Producto.NotFound", $"Producto con ID '{id}' no encontrado");

    public static Error SkuDuplicado(string sku) =>
        Error.Conflict("Producto.SkuDuplicado", $"Ya existe un producto con SKU '{sku}'");

    public static Error Inactivo(Guid id) =>
        Error.Validation("Producto.Inactivo", $"Producto '{id}' está inactivo");
}

public static class AlmacenErrors
{
    public static Error NotFound(Guid id) =>
        Error.NotFound("Almacen.NotFound", $"Almacén con ID '{id}' no encontrado");

    public static Error NombreDuplicado(string nombre) =>
        Error.Conflict("Almacen.NombreDuplicado", $"Ya existe un almacén con nombre '{nombre}'");
}

public static class LoteErrors
{
    public static Error NotFound(Guid id) =>
        Error.NotFound("Lote.NotFound", $"Lote con ID '{id}' no encontrado");

    public static Error CantidadInsuficiente(Guid id, decimal disponible, decimal solicitada) =>
        Error.Validation("Lote.CantidadInsuficiente",
            $"Lote '{id}': disponible {disponible}, solicitada {solicitada}");

    public static Error Bloqueado(Guid id) =>
        Error.Validation("Lote.Bloqueado", $"Lote '{id}' está bloqueado");
}

public static class MovimientoErrors
{
    public static Error CantidadInvalida =>
        Error.Validation("Movimiento.CantidadInvalida", "La cantidad debe ser mayor que cero");
}

public static class CategoriaErrors
{
    public static Error NotFound(Guid id) =>
        Error.NotFound("Categoria.NotFound", $"Categoría con ID '{id}' no encontrada");
}
