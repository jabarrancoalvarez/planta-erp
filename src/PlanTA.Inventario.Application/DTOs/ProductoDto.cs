using PlanTA.Inventario.Domain.Enums;

namespace PlanTA.Inventario.Application.DTOs;

public record ProductoDto(
    Guid Id,
    string SKU,
    string Nombre,
    string? Descripcion,
    TipoProducto Tipo,
    UnidadMedida UnidadMedida,
    Guid? CategoriaId,
    decimal PrecioCompra,
    decimal PrecioVenta,
    decimal PesoKg,
    string? CodigoBarras,
    string? ImagenUrl,
    bool Activo,
    DateTimeOffset CreatedAt);

public record ProductoListDto(
    Guid Id,
    string SKU,
    string Nombre,
    TipoProducto Tipo,
    UnidadMedida UnidadMedida,
    decimal PrecioVenta,
    bool Activo);
