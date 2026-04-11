using MediatR;
using Microsoft.EntityFrameworkCore;
using PlanTA.Ventas.Application.Interfaces;
using PlanTA.Ventas.Domain.Entities;
using PlanTA.Ventas.Domain.Errors;
using PlanTA.SharedKernel;

namespace PlanTA.Ventas.Application.Features.Clientes.UpdateCliente;

public sealed class UpdateClienteCommandHandler(
    IVentasDbContext db,
    ICurrentTenant tenant)
    : IRequestHandler<UpdateClienteCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(UpdateClienteCommand request, CancellationToken cancellationToken)
    {
        var cliente = await db.Clientes
            .FirstOrDefaultAsync(c => c.Id == new ClienteId(request.ClienteId) && c.EmpresaId == tenant.EmpresaId,
                cancellationToken);

        if (cliente is null)
            return Result<Guid>.Failure(ClienteErrors.NotFound(request.ClienteId));

        cliente.Actualizar(
            request.RazonSocial, request.Email,
            request.DireccionEnvio, request.DireccionFacturacion,
            request.Ciudad, request.CodigoPostal, request.Pais,
            request.Telefono, request.Web);

        await db.SaveChangesAsync(cancellationToken);
        return Result<Guid>.Success(cliente.Id.Value);
    }
}
