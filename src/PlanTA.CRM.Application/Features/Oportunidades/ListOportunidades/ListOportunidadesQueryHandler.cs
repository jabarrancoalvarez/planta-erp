using MediatR;
using Microsoft.EntityFrameworkCore;
using PlanTA.CRM.Application.DTOs;
using PlanTA.CRM.Application.Interfaces;
using PlanTA.SharedKernel;

namespace PlanTA.CRM.Application.Features.Oportunidades.ListOportunidades;

public sealed class ListOportunidadesQueryHandler(
    ICrmDbContext db,
    ICurrentTenant tenant)
    : IRequestHandler<ListOportunidadesQuery, Result<PagedResult<OportunidadListDto>>>
{
    public async Task<Result<PagedResult<OportunidadListDto>>> Handle(
        ListOportunidadesQuery request, CancellationToken ct)
    {
        var query = db.Oportunidades.AsNoTracking()
            .Where(o => o.EmpresaId == tenant.EmpresaId);

        if (request.Fase.HasValue)
            query = query.Where(o => o.Fase == request.Fase.Value);

        var total = await query.CountAsync(ct);
        var items = await query
            .OrderByDescending(o => o.CreatedAt)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(o => new OportunidadListDto(
                o.Id.Value, o.Titulo, o.ClienteId, o.Fase,
                o.ImporteEstimado, o.ProbabilidadPct,
                o.ImporteEstimado * o.ProbabilidadPct / 100m,
                o.FechaCierreEstimada, o.Descripcion))
            .ToListAsync(ct);

        return Result<PagedResult<OportunidadListDto>>.Success(
            new PagedResult<OportunidadListDto>(items, total, request.Page, request.PageSize));
    }
}
