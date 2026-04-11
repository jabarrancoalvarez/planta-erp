using MediatR;
using Microsoft.EntityFrameworkCore;
using PlanTA.Compras.Application.Interfaces;
using PlanTA.Compras.Domain.Entities;
using PlanTA.Compras.Domain.Errors;
using PlanTA.SharedKernel;

namespace PlanTA.Compras.Application.Features.OrdenesCompra.CreateOC;

public sealed class CreateOCCommandHandler(
    IComprasDbContext db,
    ICurrentTenant tenant)
    : IRequestHandler<CreateOCCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(CreateOCCommand request, CancellationToken cancellationToken)
    {
        var codigoNormalizado = request.Codigo.Trim().ToUpperInvariant();

        var codigoExists = await db.OrdenesCompra
            .AnyAsync(oc => oc.Codigo == codigoNormalizado && oc.EmpresaId == tenant.EmpresaId, cancellationToken);

        if (codigoExists)
            return Result<Guid>.Failure(OrdenCompraErrors.CodigoDuplicado(request.Codigo));

        var proveedorExists = await db.Proveedores
            .AnyAsync(p => p.Id == new ProveedorId(request.ProveedorId) && p.EmpresaId == tenant.EmpresaId, cancellationToken);

        if (!proveedorExists)
            return Result<Guid>.Failure(ProveedorErrors.NotFound(request.ProveedorId));

        var oc = OrdenCompra.Crear(
            request.Codigo,
            new ProveedorId(request.ProveedorId),
            tenant.EmpresaId,
            request.FechaEntregaEstimada,
            request.Observaciones);

        db.OrdenesCompra.Add(oc);
        await db.SaveChangesAsync(cancellationToken);

        return Result<Guid>.Success(oc.Id.Value);
    }
}
