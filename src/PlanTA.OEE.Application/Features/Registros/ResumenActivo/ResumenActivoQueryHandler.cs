using MediatR;
using Microsoft.EntityFrameworkCore;
using PlanTA.OEE.Application.DTOs;
using PlanTA.OEE.Application.Interfaces;
using PlanTA.SharedKernel;

namespace PlanTA.OEE.Application.Features.Registros.ResumenActivo;

public sealed class ResumenActivoQueryHandler(
    IOEEDbContext db,
    ICurrentTenant tenant)
    : IRequestHandler<ResumenActivoQuery, Result<ResumenOEEPorActivoDto>>
{
    public async Task<Result<ResumenOEEPorActivoDto>> Handle(
        ResumenActivoQuery request, CancellationToken ct)
    {
        var query = db.Registros.AsNoTracking()
            .Where(r => r.EmpresaId == tenant.EmpresaId && r.ActivoId == request.ActivoId);

        if (request.Desde.HasValue) query = query.Where(r => r.Fecha >= request.Desde.Value);
        if (request.Hasta.HasValue) query = query.Where(r => r.Fecha <= request.Hasta.Value);

        var rows = await query
            .Select(r => new { r.Disponibilidad, r.Rendimiento, r.Calidad, r.OEE })
            .ToListAsync(ct);

        if (rows.Count == 0)
            return Result<ResumenOEEPorActivoDto>.Success(
                new ResumenOEEPorActivoDto(request.ActivoId, 0, 0, 0, 0, 0));

        return Result<ResumenOEEPorActivoDto>.Success(new ResumenOEEPorActivoDto(
            request.ActivoId,
            rows.Count,
            Math.Round(rows.Average(r => r.Disponibilidad), 4),
            Math.Round(rows.Average(r => r.Rendimiento), 4),
            Math.Round(rows.Average(r => r.Calidad), 4),
            Math.Round(rows.Average(r => r.OEE), 4)));
    }
}
