using MediatR;
using Microsoft.EntityFrameworkCore;
using PlanTA.Facturacion.Application.DTOs;
using PlanTA.Facturacion.Application.Interfaces;
using PlanTA.SharedKernel;

namespace PlanTA.Facturacion.Application.Features.Facturas.ListFacturas;

public sealed class ListFacturasQueryHandler(
    IFacturacionDbContext db,
    ICurrentTenant tenant)
    : IRequestHandler<ListFacturasQuery, Result<PagedResult<FacturaListDto>>>
{
    public async Task<Result<PagedResult<FacturaListDto>>> Handle(
        ListFacturasQuery request, CancellationToken ct)
    {
        var query = db.Facturas.AsNoTracking()
            .Where(f => f.EmpresaId == tenant.EmpresaId);

        if (request.Estado.HasValue)
            query = query.Where(f => f.Estado == request.Estado.Value);
        if (request.ClienteId.HasValue)
            query = query.Where(f => f.ClienteId == request.ClienteId.Value);

        var total = await query.CountAsync(ct);
        var items = await query
            .OrderByDescending(f => f.FechaEmision)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(f => new FacturaListDto(
                f.Id.Value, f.NumeroCompleto, f.ClienteId, f.ClienteNombre,
                f.FechaEmision, f.Total, f.Estado, f.EstadoVerifactu))
            .ToListAsync(ct);

        return Result<PagedResult<FacturaListDto>>.Success(
            new PagedResult<FacturaListDto>(items, total, request.Page, request.PageSize));
    }
}
