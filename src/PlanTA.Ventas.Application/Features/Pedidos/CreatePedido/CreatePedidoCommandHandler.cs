using MediatR;
using Microsoft.EntityFrameworkCore;
using PlanTA.Ventas.Application.Interfaces;
using PlanTA.Ventas.Domain.Entities;
using PlanTA.Ventas.Domain.Errors;
using PlanTA.SharedKernel;

namespace PlanTA.Ventas.Application.Features.Pedidos.CreatePedido;

public sealed class CreatePedidoCommandHandler(
    IVentasDbContext db,
    ICurrentTenant tenant)
    : IRequestHandler<CreatePedidoCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(CreatePedidoCommand request, CancellationToken cancellationToken)
    {
        var codigoNormalizado = request.Codigo.Trim().ToUpperInvariant();

        var codigoExists = await db.Pedidos
            .AnyAsync(p => p.Codigo == codigoNormalizado && p.EmpresaId == tenant.EmpresaId, cancellationToken);

        if (codigoExists)
            return Result<Guid>.Failure(PedidoErrors.CodigoDuplicado(request.Codigo));

        var clienteExists = await db.Clientes
            .AnyAsync(c => c.Id == new ClienteId(request.ClienteId) && c.EmpresaId == tenant.EmpresaId, cancellationToken);

        if (!clienteExists)
            return Result<Guid>.Failure(ClienteErrors.NotFound(request.ClienteId));

        var pedido = Pedido.Crear(
            request.Codigo,
            new ClienteId(request.ClienteId),
            tenant.EmpresaId,
            request.FechaEntregaEstimada,
            request.DireccionEntrega,
            request.Observaciones);

        db.Pedidos.Add(pedido);
        await db.SaveChangesAsync(cancellationToken);

        return Result<Guid>.Success(pedido.Id.Value);
    }
}
