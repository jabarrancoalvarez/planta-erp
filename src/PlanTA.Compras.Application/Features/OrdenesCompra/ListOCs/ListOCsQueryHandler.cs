using MediatR;
using Microsoft.EntityFrameworkCore;
using PlanTA.Compras.Application.DTOs;
using PlanTA.Compras.Application.Interfaces;
using PlanTA.SharedKernel;

namespace PlanTA.Compras.Application.Features.OrdenesCompra.ListOCs;

public sealed class ListOCsQueryHandler(
    IComprasDbContext db,
    ICurrentTenant tenant)
    : IRequestHandler<ListOCsQuery, Result<PagedResult<OCListDto>>>
{
    public async Task<Result<PagedResult<OCListDto>>> Handle(
        ListOCsQuery request, CancellationToken cancellationToken)
    {
        var query = db.OrdenesCompra
            .AsNoTracking()
            .Where(oc => oc.EmpresaId == tenant.EmpresaId);

        if (request.Estado.HasValue)
            query = query.Where(oc => oc.EstadoOC == request.Estado.Value);

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var search = request.Search.ToLowerInvariant();
            query = query.Where(oc => oc.Codigo.ToLower().Contains(search));
        }

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderByDescending(oc => oc.FechaEmision)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(oc => new OCListDto(
                oc.Id.Value,
                oc.Codigo,
                oc.ProveedorId.Value,
                db.Proveedores.Where(p => p.Id == oc.ProveedorId).Select(p => p.RazonSocial).FirstOrDefault() ?? string.Empty,
                oc.FechaEmision,
                oc.EstadoOC,
                oc.Lineas.Sum(l => l.Cantidad * l.PrecioUnitario)))
            .ToListAsync(cancellationToken);

        return Result<PagedResult<OCListDto>>.Success(
            new PagedResult<OCListDto>(items, totalCount, request.Page, request.PageSize));
    }
}
