using MediatR;
using Microsoft.EntityFrameworkCore;
using PlanTA.Costes.Application.DTOs;
using PlanTA.Costes.Application.Interfaces;
using PlanTA.SharedKernel;

namespace PlanTA.Costes.Application.Features.Imputaciones.ListImputaciones;

public sealed class ListImputacionesQueryHandler(
    ICostesDbContext db,
    ICurrentTenant tenant)
    : IRequestHandler<ListImputacionesQuery, Result<PagedResult<ImputacionCosteDto>>>
{
    public async Task<Result<PagedResult<ImputacionCosteDto>>> Handle(
        ListImputacionesQuery request, CancellationToken ct)
    {
        var query = db.Imputaciones.AsNoTracking()
            .Where(i => i.EmpresaId == tenant.EmpresaId);

        if (request.OrdenFabricacionId.HasValue)
            query = query.Where(i => i.OrdenFabricacionId == request.OrdenFabricacionId.Value);
        if (request.OrdenTrabajoId.HasValue)
            query = query.Where(i => i.OrdenTrabajoId == request.OrdenTrabajoId.Value);

        var total = await query.CountAsync(ct);

        var items = await query
            .OrderByDescending(i => i.Fecha)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(i => new ImputacionCosteDto(
                i.Id.Value, i.OrdenFabricacionId, i.OrdenTrabajoId, i.ProductoId,
                i.Tipo, i.Origen, i.Cantidad, i.PrecioUnitario, i.Importe,
                i.Concepto, i.Fecha))
            .ToListAsync(ct);

        return Result<PagedResult<ImputacionCosteDto>>.Success(
            new PagedResult<ImputacionCosteDto>(items, total, request.Page, request.PageSize));
    }
}
