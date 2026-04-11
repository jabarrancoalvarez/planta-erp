using MediatR;
using Microsoft.EntityFrameworkCore;
using PlanTA.Compras.Application.Interfaces;
using PlanTA.Compras.Domain.Entities;
using PlanTA.Compras.Domain.Errors;
using PlanTA.Compras.Domain.ValueObjects;
using PlanTA.SharedKernel;

namespace PlanTA.Compras.Application.Features.Proveedores.CreateProveedor;

public sealed class CreateProveedorCommandHandler(
    IComprasDbContext db,
    ICurrentTenant tenant)
    : IRequestHandler<CreateProveedorCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(CreateProveedorCommand request, CancellationToken cancellationToken)
    {
        var nifNormalizado = request.NIF.Trim().ToUpperInvariant();

        var nifExists = await db.Proveedores
            .AnyAsync(p => p.NIF == nifNormalizado && p.EmpresaId == tenant.EmpresaId, cancellationToken);

        if (nifExists)
            return Result<Guid>.Failure(ProveedorErrors.NifDuplicado(request.NIF));

        var condicionesPago = new CondicionesPago(
            request.DiasVencimiento, request.DescuentoProntoPago, request.MetodoPago);

        var proveedor = Proveedor.Crear(
            request.RazonSocial, request.NIF, request.Email, condicionesPago, tenant.EmpresaId,
            request.Direccion, request.Ciudad, request.CodigoPostal, request.Pais,
            request.Telefono, request.Web);

        db.Proveedores.Add(proveedor);
        await db.SaveChangesAsync(cancellationToken);

        return Result<Guid>.Success(proveedor.Id.Value);
    }
}
