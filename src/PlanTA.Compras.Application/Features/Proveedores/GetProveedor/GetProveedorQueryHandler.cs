using MediatR;
using Microsoft.EntityFrameworkCore;
using PlanTA.Compras.Application.DTOs;
using PlanTA.Compras.Application.Interfaces;
using PlanTA.Compras.Domain.Entities;
using PlanTA.Compras.Domain.Errors;
using PlanTA.SharedKernel;

namespace PlanTA.Compras.Application.Features.Proveedores.GetProveedor;

public sealed class GetProveedorQueryHandler(
    IComprasDbContext db,
    ICurrentTenant tenant)
    : IRequestHandler<GetProveedorQuery, Result<ProveedorDetailDto>>
{
    public async Task<Result<ProveedorDetailDto>> Handle(GetProveedorQuery request, CancellationToken cancellationToken)
    {
        var proveedor = await db.Proveedores
            .AsNoTracking()
            .Include(p => p.Contactos)
            .Where(p => p.Id == new ProveedorId(request.ProveedorId) && p.EmpresaId == tenant.EmpresaId)
            .FirstOrDefaultAsync(cancellationToken);

        if (proveedor is null)
            return Result<ProveedorDetailDto>.Failure(ProveedorErrors.NotFound(request.ProveedorId));

        var dto = new ProveedorDetailDto(
            proveedor.Id.Value,
            proveedor.RazonSocial,
            proveedor.NIF,
            proveedor.Direccion,
            proveedor.Ciudad,
            proveedor.CodigoPostal,
            proveedor.Pais,
            proveedor.Email,
            proveedor.Telefono,
            proveedor.Web,
            proveedor.CondicionesPago.DiasVencimiento,
            proveedor.CondicionesPago.DescuentoProntoPago,
            proveedor.CondicionesPago.MetodoPago,
            proveedor.Activo,
            proveedor.CreatedAt,
            proveedor.Contactos.Select(c => new ContactoProveedorDto(
                c.Id.Value, c.Nombre, c.Cargo, c.Email, c.Telefono, c.EsPrincipal)).ToList());

        return Result<ProveedorDetailDto>.Success(dto);
    }
}
