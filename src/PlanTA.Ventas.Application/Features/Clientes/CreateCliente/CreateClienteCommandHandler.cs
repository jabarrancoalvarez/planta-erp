using MediatR;
using Microsoft.EntityFrameworkCore;
using PlanTA.Ventas.Application.Interfaces;
using PlanTA.Ventas.Domain.Entities;
using PlanTA.Ventas.Domain.Errors;
using PlanTA.SharedKernel;

namespace PlanTA.Ventas.Application.Features.Clientes.CreateCliente;

public sealed class CreateClienteCommandHandler(
    IVentasDbContext db,
    ICurrentTenant tenant)
    : IRequestHandler<CreateClienteCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(CreateClienteCommand request, CancellationToken cancellationToken)
    {
        var nifNormalizado = request.NIF.Trim().ToUpperInvariant();

        var nifExists = await db.Clientes
            .AnyAsync(c => c.NIF == nifNormalizado && c.EmpresaId == tenant.EmpresaId, cancellationToken);

        if (nifExists)
            return Result<Guid>.Failure(ClienteErrors.NifDuplicado(request.NIF));

        var cliente = Cliente.Crear(
            request.RazonSocial, request.NIF, request.Email, tenant.EmpresaId,
            request.DireccionEnvio, request.DireccionFacturacion,
            request.Ciudad, request.CodigoPostal, request.Pais,
            request.Telefono, request.Web);

        db.Clientes.Add(cliente);
        await db.SaveChangesAsync(cancellationToken);

        return Result<Guid>.Success(cliente.Id.Value);
    }
}
