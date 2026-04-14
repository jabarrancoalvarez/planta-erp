using MediatR;
using Microsoft.EntityFrameworkCore;
using PlanTA.Activos.Application.DTOs;
using PlanTA.Activos.Application.Interfaces;
using PlanTA.SharedKernel;

namespace PlanTA.Activos.Application.Features.Activos.ListActivos;

public sealed class ListActivosQueryHandler(
    IActivosDbContext db,
    ICurrentTenant tenant)
    : IRequestHandler<ListActivosQuery, Result<PagedResult<ActivoListDto>>>
{
    public async Task<Result<PagedResult<ActivoListDto>>> Handle(
        ListActivosQuery request, CancellationToken cancellationToken)
    {
        var query = db.Activos.AsNoTracking()
            .Where(a => a.EmpresaId == tenant.EmpresaId);

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var s = request.Search.ToLowerInvariant();
            query = query.Where(a => a.Nombre.ToLower().Contains(s) || a.Codigo.ToLower().Contains(s));
        }

        if (request.Tipo.HasValue) query = query.Where(a => a.Tipo == request.Tipo.Value);
        if (request.Estado.HasValue) query = query.Where(a => a.Estado == request.Estado.Value);
        if (request.Criticidad.HasValue) query = query.Where(a => a.Criticidad == request.Criticidad.Value);

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderBy(a => a.Codigo)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(a => new ActivoListDto(
                a.Id.Value, a.Codigo, a.Nombre, a.Tipo, a.Criticidad, a.Estado, a.Ubicacion))
            .ToListAsync(cancellationToken);

        return Result<PagedResult<ActivoListDto>>.Success(
            new PagedResult<ActivoListDto>(items, totalCount, request.Page, request.PageSize));
    }
}
