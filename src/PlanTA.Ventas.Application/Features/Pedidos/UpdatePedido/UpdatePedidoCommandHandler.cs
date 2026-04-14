using MediatR;
using Microsoft.EntityFrameworkCore;
using PlanTA.Ventas.Application.Interfaces;
using PlanTA.Ventas.Domain.Entities;
using PlanTA.Ventas.Domain.Errors;
using PlanTA.SharedKernel;

namespace PlanTA.Ventas.Application.Features.Pedidos.UpdatePedido;

public sealed class UpdatePedidoCommandHandler(
    IVentasDbContext db,
    ICurrentTenant tenant)
    : IRequestHandler<UpdatePedidoCommand, Result<bool>>
{
    public async Task<Result<bool>> Handle(UpdatePedidoCommand request, CancellationToken cancellationToken)
    {
        var pedido = await db.Pedidos
            .Where(p => p.Id == new PedidoId(request.PedidoId) && p.EmpresaId == tenant.EmpresaId)
            .FirstOrDefaultAsync(cancellationToken);

        if (pedido is null)
            return Result<bool>.Failure(PedidoErrors.NotFound(request.PedidoId));

        var result = pedido.Editar(request.FechaEntregaEstimada, request.DireccionEntrega, request.Observaciones);
        if (result.IsFailure)
            return result;

        await db.SaveChangesAsync(cancellationToken);
        return Result<bool>.Success(true);
    }
}
