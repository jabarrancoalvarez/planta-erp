using MediatR;
using PlanTA.SharedKernel;

namespace PlanTA.Inventario.Application.Features.Productos.DeleteProducto;

public record DeleteProductoCommand(Guid ProductoId) : IRequest<Result<Guid>>;
