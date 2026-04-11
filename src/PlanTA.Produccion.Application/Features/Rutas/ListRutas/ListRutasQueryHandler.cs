using MediatR;
using Microsoft.EntityFrameworkCore;
using PlanTA.Produccion.Application.DTOs;
using PlanTA.Produccion.Application.Interfaces;
using PlanTA.SharedKernel;

namespace PlanTA.Produccion.Application.Features.Rutas.ListRutas;

public sealed class ListRutasQueryHandler(
    IProduccionDbContext db,
    ICurrentTenant tenant)
    : IRequestHandler<ListRutasQuery, Result<PagedResult<RutaProduccionListDto>>>
{
    public async Task<Result<PagedResult<RutaProduccionListDto>>> Handle(
        ListRutasQuery request, CancellationToken cancellationToken)
    {
        var query = db.RutasProduccion
            .AsNoTracking()
            .Where(r => r.EmpresaId == tenant.EmpresaId);

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var search = request.Search.ToLowerInvariant();
            query = query.Where(r => r.Nombre.ToLower().Contains(search));
        }

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderBy(r => r.Nombre)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(r => new RutaProduccionListDto(
                r.Id.Value,
                r.ProductoId,
                r.Nombre,
                r.Activa,
                r.Operaciones.Count))
            .ToListAsync(cancellationToken);

        return Result<PagedResult<RutaProduccionListDto>>.Success(
            new PagedResult<RutaProduccionListDto>(items, totalCount, request.Page, request.PageSize));
    }
}
