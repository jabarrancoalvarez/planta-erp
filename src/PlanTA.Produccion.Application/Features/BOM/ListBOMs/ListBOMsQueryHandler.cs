using MediatR;
using Microsoft.EntityFrameworkCore;
using PlanTA.Produccion.Application.DTOs;
using PlanTA.Produccion.Application.Interfaces;
using PlanTA.SharedKernel;

namespace PlanTA.Produccion.Application.Features.BOM.ListBOMs;

public sealed class ListBOMsQueryHandler(
    IProduccionDbContext db,
    ICurrentTenant tenant)
    : IRequestHandler<ListBOMsQuery, Result<PagedResult<ListaMaterialesListDto>>>
{
    public async Task<Result<PagedResult<ListaMaterialesListDto>>> Handle(
        ListBOMsQuery request, CancellationToken cancellationToken)
    {
        var query = db.ListasMateriales
            .AsNoTracking()
            .Where(b => b.EmpresaId == tenant.EmpresaId);

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var search = request.Search.ToLowerInvariant();
            query = query.Where(b => b.Nombre.ToLower().Contains(search));
        }

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderBy(b => b.Nombre)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(b => new ListaMaterialesListDto(
                b.Id.Value,
                b.ProductoId,
                b.Nombre,
                b.VersionBOM,
                b.Activo,
                b.Lineas.Count))
            .ToListAsync(cancellationToken);

        return Result<PagedResult<ListaMaterialesListDto>>.Success(
            new PagedResult<ListaMaterialesListDto>(items, totalCount, request.Page, request.PageSize));
    }
}
