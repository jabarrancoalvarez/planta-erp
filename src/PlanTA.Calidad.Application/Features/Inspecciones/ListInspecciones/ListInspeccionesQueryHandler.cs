using MediatR;
using Microsoft.EntityFrameworkCore;
using PlanTA.Calidad.Application.DTOs;
using PlanTA.Calidad.Application.Interfaces;
using PlanTA.SharedKernel;

namespace PlanTA.Calidad.Application.Features.Inspecciones.ListInspecciones;

public sealed class ListInspeccionesQueryHandler(
    ICalidadDbContext db,
    ICurrentTenant tenant)
    : IRequestHandler<ListInspeccionesQuery, Result<PagedResult<InspeccionListDto>>>
{
    public async Task<Result<PagedResult<InspeccionListDto>>> Handle(
        ListInspeccionesQuery request, CancellationToken cancellationToken)
    {
        var query = db.Inspecciones
            .AsNoTracking()
            .Where(i => i.EmpresaId == tenant.EmpresaId);

        if (request.Origen.HasValue)
            query = query.Where(i => i.OrigenInspeccion == request.Origen.Value);

        if (request.Resultado.HasValue)
            query = query.Where(i => i.ResultadoInspeccion == request.Resultado.Value);

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderByDescending(i => i.FechaInspeccion)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(i => new InspeccionListDto(
                i.Id.Value, i.OrigenInspeccion, i.ReferenciaOrigenId,
                i.FechaInspeccion, i.ResultadoInspeccion))
            .ToListAsync(cancellationToken);

        return Result<PagedResult<InspeccionListDto>>.Success(
            new PagedResult<InspeccionListDto>(items, totalCount, request.Page, request.PageSize));
    }
}
