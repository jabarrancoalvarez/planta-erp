using MediatR;
using Microsoft.EntityFrameworkCore;
using PlanTA.Compras.Application.Interfaces;
using PlanTA.Compras.Domain.Entities;
using PlanTA.Compras.Domain.Errors;
using PlanTA.Compras.Domain.ValueObjects;
using PlanTA.SharedKernel;

namespace PlanTA.Compras.Application.Features.Proveedores.UpdateProveedor;

public sealed class UpdateProveedorCommandHandler(
    IComprasDbContext db,
    ICurrentTenant tenant)
    : IRequestHandler<UpdateProveedorCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(UpdateProveedorCommand request, CancellationToken cancellationToken)
    {
        var proveedor = await db.Proveedores
            .FirstOrDefaultAsync(p => p.Id == new ProveedorId(request.ProveedorId) && p.EmpresaId == tenant.EmpresaId,
                cancellationToken);

        if (proveedor is null)
            return Result<Guid>.Failure(ProveedorErrors.NotFound(request.ProveedorId));

        var condicionesPago = new CondicionesPago(
            request.DiasVencimiento, request.DescuentoProntoPago, request.MetodoPago);

        proveedor.Actualizar(
            request.RazonSocial, request.Email, condicionesPago,
            request.Direccion, request.Ciudad, request.CodigoPostal, request.Pais,
            request.Telefono, request.Web);

        await db.SaveChangesAsync(cancellationToken);
        return Result<Guid>.Success(proveedor.Id.Value);
    }
}
