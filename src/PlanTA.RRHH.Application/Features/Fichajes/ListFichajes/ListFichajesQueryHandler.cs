using MediatR;
using Microsoft.EntityFrameworkCore;
using PlanTA.RRHH.Application.DTOs;
using PlanTA.RRHH.Application.Interfaces;
using PlanTA.RRHH.Domain.Entities;
using PlanTA.SharedKernel;

namespace PlanTA.RRHH.Application.Features.Fichajes.ListFichajes;

public sealed class ListFichajesQueryHandler(
    IRRHHDbContext db,
    ICurrentTenant tenant)
    : IRequestHandler<ListFichajesQuery, Result<PagedResult<FichajeDto>>>
{
    public async Task<Result<PagedResult<FichajeDto>>> Handle(
        ListFichajesQuery request, CancellationToken ct)
    {
        var query = db.Fichajes.AsNoTracking()
            .Where(f => f.EmpresaId == tenant.EmpresaId);

        if (request.EmpleadoId.HasValue)
        {
            var eid = new EmpleadoId(request.EmpleadoId.Value);
            query = query.Where(f => f.EmpleadoId == eid);
        }
        if (request.Desde.HasValue)
            query = query.Where(f => f.Momento >= request.Desde.Value);
        if (request.Hasta.HasValue)
            query = query.Where(f => f.Momento <= request.Hasta.Value);

        var total = await query.CountAsync(ct);

        var items = await (
            from f in query
            join e in db.Empleados.AsNoTracking() on f.EmpleadoId equals e.Id
            orderby f.Momento descending
            select new FichajeDto(
                f.Id.Value, e.Id.Value, (e.Nombre + " " + e.Apellidos),
                f.Tipo, f.Momento, f.ActivoId, f.OrdenFabricacionId, f.OrdenTrabajoId, f.Notas))
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync(ct);

        return Result<PagedResult<FichajeDto>>.Success(
            new PagedResult<FichajeDto>(items, total, request.Page, request.PageSize));
    }
}
