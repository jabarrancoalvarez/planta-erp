using MediatR;
using Microsoft.EntityFrameworkCore;
using PlanTA.Ventas.Application.Interfaces;
using PlanTA.Ventas.Domain.Entities;
using PlanTA.Ventas.Domain.Errors;
using PlanTA.SharedKernel;

namespace PlanTA.Ventas.Application.Features.Pedidos.AddLineaPedido;

public sealed class AddLineaPedidoCommandHandler(
    IVentasDbContext db,
    ICurrentTenant tenant)
    : IRequestHandler<AddLineaPedidoCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(AddLineaPedidoCommand request, CancellationToken cancellationToken)
    {
        var pedido = await db.Pedidos
            .Include(p => p.Lineas)
            .Where(p => p.Id == new PedidoId(request.PedidoId) && p.EmpresaId == tenant.EmpresaId)
            .FirstOrDefaultAsync(cancellationToken);

        if (pedido is null)
            return Result<Guid>.Failure(PedidoErrors.NotFound(request.PedidoId));

        var result = pedido.AgregarLinea(
            request.ProductoId, request.Descripcion, request.Cantidad,
            request.PrecioUnitario, request.Descuento);

        if (result.IsFailure)
            return Result<Guid>.Failure(result.Error!);

        await db.SaveChangesAsync(cancellationToken);

        return Result<Guid>.Success(result.Value!.Id.Value);
    }
}
