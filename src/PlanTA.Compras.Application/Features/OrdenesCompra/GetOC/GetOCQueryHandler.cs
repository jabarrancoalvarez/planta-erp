using MediatR;
using Microsoft.EntityFrameworkCore;
using PlanTA.Compras.Application.DTOs;
using PlanTA.Compras.Application.Interfaces;
using PlanTA.Compras.Domain.Entities;
using PlanTA.Compras.Domain.Errors;
using PlanTA.SharedKernel;

namespace PlanTA.Compras.Application.Features.OrdenesCompra.GetOC;

public sealed class GetOCQueryHandler(
    IComprasDbContext db,
    ICurrentTenant tenant)
    : IRequestHandler<GetOCQuery, Result<OCDetailDto>>
{
    public async Task<Result<OCDetailDto>> Handle(GetOCQuery request, CancellationToken cancellationToken)
    {
        var oc = await db.OrdenesCompra
            .AsNoTracking()
            .Include(o => o.Lineas)
            .Where(o => o.Id == new OrdenCompraId(request.OrdenCompraId) && o.EmpresaId == tenant.EmpresaId)
            .FirstOrDefaultAsync(cancellationToken);

        if (oc is null)
            return Result<OCDetailDto>.Failure(OrdenCompraErrors.NotFound(request.OrdenCompraId));

        var proveedor = await db.Proveedores
            .AsNoTracking()
            .Where(p => p.Id == oc.ProveedorId)
            .Select(p => p.RazonSocial)
            .FirstOrDefaultAsync(cancellationToken);

        var dto = new OCDetailDto(
            oc.Id.Value,
            oc.Codigo,
            oc.ProveedorId.Value,
            proveedor ?? string.Empty,
            oc.FechaEmision,
            oc.FechaEntregaEstimada,
            oc.EstadoOC,
            oc.Observaciones,
            oc.Total,
            oc.CreatedAt,
            oc.Lineas.Select(l => new LineaOCDto(
                l.Id.Value, l.ProductoId, l.Descripcion,
                l.Cantidad, l.PrecioUnitario, l.CantidadRecibida, l.Total)).ToList());

        return Result<OCDetailDto>.Success(dto);
    }
}
