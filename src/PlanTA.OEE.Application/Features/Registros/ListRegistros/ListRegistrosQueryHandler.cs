using MediatR;
using Microsoft.EntityFrameworkCore;
using PlanTA.OEE.Application.DTOs;
using PlanTA.OEE.Application.Interfaces;
using PlanTA.SharedKernel;

namespace PlanTA.OEE.Application.Features.Registros.ListRegistros;

public sealed class ListRegistrosQueryHandler(
    IOEEDbContext db,
    ICurrentTenant tenant)
    : IRequestHandler<ListRegistrosQuery, Result<PagedResult<RegistroOEEDto>>>
{
    public async Task<Result<PagedResult<RegistroOEEDto>>> Handle(
        ListRegistrosQuery request, CancellationToken ct)
    {
        var query = db.Registros.AsNoTracking()
            .Where(r => r.EmpresaId == tenant.EmpresaId);

        if (request.ActivoId.HasValue)
            query = query.Where(r => r.ActivoId == request.ActivoId.Value);
        if (request.Desde.HasValue)
            query = query.Where(r => r.Fecha >= request.Desde.Value);
        if (request.Hasta.HasValue)
            query = query.Where(r => r.Fecha <= request.Hasta.Value);

        var total = await query.CountAsync(ct);
        var items = await query
            .OrderByDescending(r => r.Fecha)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(r => new RegistroOEEDto(
                r.Id.Value, r.ActivoId, r.TurnoId, r.OrdenFabricacionId, r.Fecha,
                r.MinutosPlanificados, r.MinutosFuncionamiento,
                r.PiezasTotales, r.PiezasBuenas, r.TiempoCicloTeoricoSeg,
                r.Disponibilidad, r.Rendimiento, r.Calidad, r.OEE, r.Notas))
            .ToListAsync(ct);

        return Result<PagedResult<RegistroOEEDto>>.Success(
            new PagedResult<RegistroOEEDto>(items, total, request.Page, request.PageSize));
    }
}
