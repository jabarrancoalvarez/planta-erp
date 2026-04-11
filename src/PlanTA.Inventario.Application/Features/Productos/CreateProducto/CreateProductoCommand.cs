using PlanTA.Inventario.Domain.Enums;
using PlanTA.SharedKernel.CQRS;

namespace PlanTA.Inventario.Application.Features.Productos.CreateProducto;

public record CreateProductoCommand(
    string SKU,
    string Nombre,
    TipoProducto Tipo,
    UnidadMedida UnidadMedida,
    string? Descripcion = null,
    Guid? CategoriaId = null,
    decimal PrecioCompra = 0,
    decimal PrecioVenta = 0) : ICommand<Guid>;
