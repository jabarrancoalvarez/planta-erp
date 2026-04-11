using PlanTA.SharedKernel.CQRS;

namespace PlanTA.Inventario.Application.Features.Productos.UpdateProducto;

public record UpdateProductoCommand(
    Guid ProductoId,
    string Nombre,
    string? Descripcion,
    decimal PrecioCompra,
    decimal PrecioVenta,
    Guid? CategoriaId) : ICommand<Guid>;
