using MediatR;
using Microsoft.EntityFrameworkCore;
using PlanTA.Incidencias.Application.DTOs;
using PlanTA.Incidencias.Application.Interfaces;
using PlanTA.Incidencias.Domain.Enums;
using PlanTA.SharedKernel;
using PlanTA.SharedKernel.CQRS;

namespace PlanTA.Incidencias.Application.Features.Incidencias.ListIncidencias;

public record ListIncidenciasQuery(
    EstadoIncidencia? Estado = null,
    SeveridadIncidencia? Severidad = null,
    TipoIncidencia? Tipo = null,
    Guid? ActivoId = null,
    int Page = 1,
    int PageSize = 20) : IQuery<PagedResult<IncidenciaListDto>>;

public sealed class ListIncidenciasQueryHandler(
    IIncidenciasDbContext db,
    ICurrentTenant tenant)
    : IRequestHandler<ListIncidenciasQuery, Result<PagedResult<IncidenciaListDto>>>
{
    public async Task<Result<PagedResult<IncidenciaListDto>>> Handle(
        ListIncidenciasQuery request, CancellationToken cancellationToken)
    {
        var q = db.Incidencias.AsNoTracking()
            .Where(i => i.EmpresaId == tenant.EmpresaId);

        if (request.Estado.HasValue) q = q.Where(i => i.Estado == request.Estado.Value);
        if (request.Severidad.HasValue) q = q.Where(i => i.Severidad == request.Severidad.Value);
        if (request.Tipo.HasValue) q = q.Where(i => i.Tipo == request.Tipo.Value);
        if (request.ActivoId.HasValue) q = q.Where(i => i.ActivoId == request.ActivoId.Value);

        var total = await q.CountAsync(cancellationToken);
        var items = await q
            .OrderByDescending(i => i.FechaApertura)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(i => new IncidenciaListDto(
                i.Id.Value, i.Codigo, i.Titulo, i.Tipo, i.Severidad, i.Estado,
                i.ActivoId, i.FechaApertura, i.ParadaLinea))
            .ToListAsync(cancellationToken);

        return Result<PagedResult<IncidenciaListDto>>.Success(
            new PagedResult<IncidenciaListDto>(items, total, request.Page, request.PageSize));
    }
}
