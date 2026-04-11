using MediatR;
using Microsoft.EntityFrameworkCore;
using PlanTA.Ventas.Application.Interfaces;
using PlanTA.Ventas.Domain.Entities;
using PlanTA.Ventas.Domain.Enums;
using PlanTA.Ventas.Domain.Errors;
using PlanTA.SharedKernel;

namespace PlanTA.Ventas.Application.Features.Expediciones.CreateExpedicion;

public sealed class CreateExpedicionCommandHandler(
    IVentasDbContext db,
    ICurrentTenant tenant)
    : IRequestHandler<CreateExpedicionCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(CreateExpedicionCommand request, CancellationToken cancellationToken)
    {
        var pedido = await db.Pedidos
            .Include(p => p.Lineas)
            .Where(p => p.Id == new PedidoId(request.PedidoId) && p.EmpresaId == tenant.EmpresaId)
            .FirstOrDefaultAsync(cancellationToken);

        if (pedido is null)
            return Result<Guid>.Failure(PedidoErrors.NotFound(request.PedidoId));

        if (pedido.EstadoPedido is not (EstadoPedido.Confirmado or EstadoPedido.EnPreparacion or EstadoPedido.ParcialmenteEnviado))
            return Result<Guid>.Failure(
                PedidoErrors.TransicionInvalida(pedido.EstadoPedido.ToString(), "Expedicion"));

        var expedicion = Expedicion.Crear(
            new PedidoId(request.PedidoId),
            tenant.EmpresaId,
            request.NumeroSeguimiento,
            request.Transportista,
            request.Observaciones);

        if (request.Lineas is not null)
        {
            foreach (var lineaReq in request.Lineas)
            {
                var lineaPedido = pedido.Lineas.FirstOrDefault(l => l.Id == new LineaPedidoId(lineaReq.LineaPedidoId));
                if (lineaPedido is null)
                    continue;

                var pendiente = lineaPedido.CantidadPendiente;
                if (lineaReq.Cantidad > pendiente)
                    return Result<Guid>.Failure(
                        ExpedicionErrors.CantidadExcedida(lineaReq.LineaPedidoId, pendiente, lineaReq.Cantidad));

                expedicion.AgregarLinea(
                    new LineaPedidoId(lineaReq.LineaPedidoId),
                    lineaReq.ProductoId,
                    lineaReq.Cantidad,
                    lineaReq.LoteOrigenId);

                lineaPedido.RegistrarEnvio(lineaReq.Cantidad);
            }
        }

        // Update Pedido state based on lines
        if (pedido.TodasLineasEnviadas())
            pedido.EnviarCompleto();
        else if (pedido.AlgunaLineaParcialmenteEnviada() || pedido.Lineas.Any(l => l.CantidadEnviada > 0))
            pedido.EnviarParcialmente();

        db.Expediciones.Add(expedicion);
        await db.SaveChangesAsync(cancellationToken);

        return Result<Guid>.Success(expedicion.Id.Value);
    }
}
