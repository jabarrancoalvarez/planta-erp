using MediatR;
using Microsoft.EntityFrameworkCore;
using PlanTA.Ventas.Application.Interfaces;
using PlanTA.Ventas.Domain.Entities;
using PlanTA.Ventas.Domain.Enums;
using PlanTA.Ventas.Domain.Errors;
using PlanTA.SharedKernel;

namespace PlanTA.Ventas.Application.Features.Pedidos.CambiarEstadoPedido;

public sealed class CambiarEstadoPedidoCommandHandler(
    IVentasDbContext db,
    ICurrentTenant tenant)
    : IRequestHandler<CambiarEstadoPedidoCommand, Result<bool>>
{
    public async Task<Result<bool>> Handle(CambiarEstadoPedidoCommand request, CancellationToken cancellationToken)
    {
        var pedido = await db.Pedidos
            .Include(p => p.Lineas)
            .Where(p => p.Id == new PedidoId(request.PedidoId) && p.EmpresaId == tenant.EmpresaId)
            .FirstOrDefaultAsync(cancellationToken);

        if (pedido is null)
            return Result<bool>.Failure(PedidoErrors.NotFound(request.PedidoId));

        var result = request.EstadoDestino switch
        {
            EstadoPedido.Confirmado => pedido.Confirmar(),
            EstadoPedido.EnPreparacion => pedido.PrepararEnvio(),
            EstadoPedido.ParcialmenteEnviado => pedido.EnviarParcialmente(),
            EstadoPedido.Enviado => pedido.EnviarCompleto(),
            EstadoPedido.Entregado => pedido.Entregar(),
            EstadoPedido.Cancelado => pedido.Cancelar(request.Motivo ?? "Sin motivo especificado"),
            _ => Result<bool>.Failure(
                PedidoErrors.TransicionInvalida(pedido.EstadoPedido.ToString(), request.EstadoDestino.ToString()))
        };

        if (result.IsFailure)
            return result;

        await db.SaveChangesAsync(cancellationToken);
        return Result<bool>.Success(true);
    }
}
