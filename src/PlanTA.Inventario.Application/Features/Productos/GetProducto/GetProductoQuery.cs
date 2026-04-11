using PlanTA.Inventario.Application.DTOs;
using PlanTA.SharedKernel.CQRS;

namespace PlanTA.Inventario.Application.Features.Productos.GetProducto;

public record GetProductoQuery(Guid ProductoId) : IQuery<ProductoDto>;
