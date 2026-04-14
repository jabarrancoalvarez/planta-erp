using MediatR;
using Microsoft.EntityFrameworkCore;
using PlanTA.RRHH.Application.DTOs;
using PlanTA.RRHH.Application.Interfaces;
using PlanTA.RRHH.Domain.Entities;
using PlanTA.SharedKernel;

namespace PlanTA.RRHH.Application.Features.Ausencias.ListAusencias;

public sealed class ListAusenciasQueryHandler(
    IRRHHDbContext db,
    ICurrentTenant tenant)
    : IRequestHandler<ListAusenciasQuery, Result<PagedResult<AusenciaDto>>>
{
    public async Task<Result<PagedResult<AusenciaDto>>> Handle(
        ListAusenciasQuery request, CancellationToken ct)
    {
        var query = db.Ausencias.AsNoTracking()
            .Where(a => a.EmpresaId == tenant.EmpresaId);

        if (request.EmpleadoId.HasValue)
        {
            var eid = new EmpleadoId(request.EmpleadoId.Value);
            query = query.Where(a => a.EmpleadoId == eid);
        }
        if (request.Estado.HasValue)
            query = query.Where(a => a.Estado == request.Estado.Value);

        var total = await query.CountAsync(ct);

        var items = await (
            from a in query
            join e in db.Empleados.AsNoTracking() on a.EmpleadoId equals e.Id
            orderby a.FechaInicio descending
            select new AusenciaDto(
                a.Id.Value, e.Id.Value, (e.Nombre + " " + e.Apellidos),
                a.Tipo, a.Estado, a.FechaInicio, a.FechaFin,
                (int)Math.Ceiling((a.FechaFin - a.FechaInicio).TotalDays) + 1,
                a.Motivo))
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync(ct);

        return Result<PagedResult<AusenciaDto>>.Success(
            new PagedResult<AusenciaDto>(items, total, request.Page, request.PageSize));
    }
}
