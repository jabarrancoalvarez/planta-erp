using MediatR;
using Microsoft.EntityFrameworkCore;
using PlanTA.Calidad.Application.DTOs;
using PlanTA.Calidad.Application.Interfaces;
using PlanTA.SharedKernel;

namespace PlanTA.Calidad.Application.Features.NoConformidades.ListNCs;

public sealed class ListNCsQueryHandler(
    ICalidadDbContext db,
    ICurrentTenant tenant)
    : IRequestHandler<ListNCsQuery, Result<PagedResult<NCListDto>>>
{
    public async Task<Result<PagedResult<NCListDto>>> Handle(
        ListNCsQuery request, CancellationToken cancellationToken)
    {
        var query = db.NoConformidades
            .AsNoTracking()
            .Where(nc => nc.EmpresaId == tenant.EmpresaId);

        if (request.Estado.HasValue)
            query = query.Where(nc => nc.EstadoNoConformidad == request.Estado.Value);

        if (request.Severidad.HasValue)
            query = query.Where(nc => nc.SeveridadNC == request.Severidad.Value);

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderByDescending(nc => nc.FechaDeteccion)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(nc => new NCListDto(
                nc.Id.Value, nc.Codigo, nc.OrigenInspeccion,
                nc.SeveridadNC, nc.EstadoNoConformidad, nc.FechaDeteccion))
            .ToListAsync(cancellationToken);

        return Result<PagedResult<NCListDto>>.Success(
            new PagedResult<NCListDto>(items, totalCount, request.Page, request.PageSize));
    }
}
