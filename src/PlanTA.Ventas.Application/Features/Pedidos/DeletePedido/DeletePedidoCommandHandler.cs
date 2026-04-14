using MediatR;
using Microsoft.EntityFrameworkCore;
using PlanTA.Ventas.Application.Interfaces;
using PlanTA.Ventas.Domain.Entities;
using PlanTA.Ventas.Domain.Errors;
using PlanTA.SharedKernel;

namespace PlanTA.Ventas.Application.Features.Pedidos.DeletePedido;

public sealed class DeletePedidoCommandHandler(
    IVentasDbContext db,
    ICurrentTenant tenant)
    : IRequestHandler<DeletePedidoCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(DeletePedidoCommand request, CancellationToken cancellationToken)
    {
        var pedido = await db.Pedidos
            .FirstOrDefaultAsync(
                p => p.Id == new PedidoId(request.PedidoId) && p.EmpresaId == tenant.EmpresaId,
                cancellationToken);

        if (pedido is null)
            return Result<Guid>.Failure(PedidoErrors.NotFound(request.PedidoId));

        pedido.SoftDelete();
        await db.SaveChangesAsync(cancellationToken);

        return Result<Guid>.Success(pedido.Id.Value);
    }
}
