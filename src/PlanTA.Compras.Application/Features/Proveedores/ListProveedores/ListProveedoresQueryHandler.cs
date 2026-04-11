using MediatR;
using Microsoft.EntityFrameworkCore;
using PlanTA.Compras.Application.DTOs;
using PlanTA.Compras.Application.Interfaces;
using PlanTA.SharedKernel;

namespace PlanTA.Compras.Application.Features.Proveedores.ListProveedores;

public sealed class ListProveedoresQueryHandler(
    IComprasDbContext db,
    ICurrentTenant tenant)
    : IRequestHandler<ListProveedoresQuery, Result<PagedResult<ProveedorListDto>>>
{
    public async Task<Result<PagedResult<ProveedorListDto>>> Handle(
        ListProveedoresQuery request, CancellationToken cancellationToken)
    {
        var query = db.Proveedores
            .AsNoTracking()
            .Where(p => p.EmpresaId == tenant.EmpresaId);

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var search = request.Search.ToLowerInvariant();
            query = query.Where(p =>
                p.RazonSocial.ToLower().Contains(search) ||
                p.NIF.ToLower().Contains(search));
        }

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderBy(p => p.RazonSocial)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(p => new ProveedorListDto(
                p.Id.Value, p.RazonSocial, p.NIF, p.Ciudad, p.Email, p.Activo))
            .ToListAsync(cancellationToken);

        return Result<PagedResult<ProveedorListDto>>.Success(
            new PagedResult<ProveedorListDto>(items, totalCount, request.Page, request.PageSize));
    }
}
