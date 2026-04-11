using MediatR;
using Microsoft.EntityFrameworkCore;
using PlanTA.Calidad.Application.DTOs;
using PlanTA.Calidad.Application.Interfaces;
using PlanTA.SharedKernel;

namespace PlanTA.Calidad.Application.Features.Plantillas.ListPlantillas;

public sealed class ListPlantillasQueryHandler(
    ICalidadDbContext db,
    ICurrentTenant tenant)
    : IRequestHandler<ListPlantillasQuery, Result<PagedResult<PlantillaListDto>>>
{
    public async Task<Result<PagedResult<PlantillaListDto>>> Handle(
        ListPlantillasQuery request, CancellationToken cancellationToken)
    {
        var query = db.PlantillasInspeccion
            .AsNoTracking()
            .Where(p => p.EmpresaId == tenant.EmpresaId);

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var search = request.Search.ToLowerInvariant();
            query = query.Where(p => p.Nombre.ToLower().Contains(search));
        }

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderBy(p => p.Nombre)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(p => new PlantillaListDto(
                p.Id.Value, p.Nombre, p.OrigenInspeccion, p.Version, p.Activa,
                p.Criterios.Count))
            .ToListAsync(cancellationToken);

        return Result<PagedResult<PlantillaListDto>>.Success(
            new PagedResult<PlantillaListDto>(items, totalCount, request.Page, request.PageSize));
    }
}
