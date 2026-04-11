using MediatR;
using Microsoft.EntityFrameworkCore;
using PlanTA.Ventas.Application.DTOs;
using PlanTA.Ventas.Application.Interfaces;
using PlanTA.Ventas.Domain.Entities;
using PlanTA.Ventas.Domain.Errors;
using PlanTA.SharedKernel;

namespace PlanTA.Ventas.Application.Features.Clientes.GetCliente;

public sealed class GetClienteQueryHandler(
    IVentasDbContext db,
    ICurrentTenant tenant)
    : IRequestHandler<GetClienteQuery, Result<ClienteDetailDto>>
{
    public async Task<Result<ClienteDetailDto>> Handle(GetClienteQuery request, CancellationToken cancellationToken)
    {
        var cliente = await db.Clientes
            .AsNoTracking()
            .Include(c => c.Contactos)
            .Where(c => c.Id == new ClienteId(request.ClienteId) && c.EmpresaId == tenant.EmpresaId)
            .FirstOrDefaultAsync(cancellationToken);

        if (cliente is null)
            return Result<ClienteDetailDto>.Failure(ClienteErrors.NotFound(request.ClienteId));

        var dto = new ClienteDetailDto(
            cliente.Id.Value,
            cliente.RazonSocial,
            cliente.NIF,
            cliente.DireccionEnvio,
            cliente.DireccionFacturacion,
            cliente.Ciudad,
            cliente.CodigoPostal,
            cliente.Pais,
            cliente.Email,
            cliente.Telefono,
            cliente.Web,
            cliente.Activo,
            cliente.CreatedAt,
            cliente.Contactos.Select(c => new ContactoClienteDto(
                c.Id.Value, c.Nombre, c.Cargo, c.Email, c.Telefono, c.EsPrincipal)).ToList());

        return Result<ClienteDetailDto>.Success(dto);
    }
}
