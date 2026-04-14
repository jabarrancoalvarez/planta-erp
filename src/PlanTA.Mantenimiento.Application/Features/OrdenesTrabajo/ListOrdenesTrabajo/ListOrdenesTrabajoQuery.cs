using MediatR;
using Microsoft.EntityFrameworkCore;
using PlanTA.Mantenimiento.Application.DTOs;
using PlanTA.Mantenimiento.Application.Interfaces;
using PlanTA.Mantenimiento.Domain.Enums;
using PlanTA.SharedKernel;
using PlanTA.SharedKernel.CQRS;

namespace PlanTA.Mantenimiento.Application.Features.OrdenesTrabajo.ListOrdenesTrabajo;

public record ListOrdenesTrabajoQuery(
    EstadoOT? Estado = null,
    TipoMantenimiento? Tipo = null,
    Guid? ActivoId = null,
    int Page = 1,
    int PageSize = 20) : IQuery<PagedResult<OrdenTrabajoListDto>>;

public sealed class ListOrdenesTrabajoQueryHandler(
    IMantenimientoDbContext db,
    ICurrentTenant tenant)
    : IRequestHandler<ListOrdenesTrabajoQuery, Result<PagedResult<OrdenTrabajoListDto>>>
{
    public async Task<Result<PagedResult<OrdenTrabajoListDto>>> Handle(
        ListOrdenesTrabajoQuery request, CancellationToken cancellationToken)
    {
        var q = db.OrdenesTrabajo.AsNoTracking()
            .Where(o => o.EmpresaId == tenant.EmpresaId);

        if (request.Estado.HasValue) q = q.Where(o => o.Estado == request.Estado.Value);
        if (request.Tipo.HasValue) q = q.Where(o => o.Tipo == request.Tipo.Value);
        if (request.ActivoId.HasValue) q = q.Where(o => o.ActivoId == request.ActivoId.Value);

        var total = await q.CountAsync(cancellationToken);
        var items = await q
            .OrderByDescending(o => o.FechaPlanificada ?? o.CreatedAt)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(o => new OrdenTrabajoListDto(
                o.Id.Value, o.Codigo, o.Titulo, o.Tipo, o.Estado, o.Prioridad, o.ActivoId, o.FechaPlanificada))
            .ToListAsync(cancellationToken);

        return Result<PagedResult<OrdenTrabajoListDto>>.Success(
            new PagedResult<OrdenTrabajoListDto>(items, total, request.Page, request.PageSize));
    }
}
