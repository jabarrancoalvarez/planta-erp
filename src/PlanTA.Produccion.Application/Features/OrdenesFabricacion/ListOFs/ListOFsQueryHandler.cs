using MediatR;
using Microsoft.EntityFrameworkCore;
using PlanTA.Produccion.Application.DTOs;
using PlanTA.Produccion.Application.Interfaces;
using PlanTA.SharedKernel;

namespace PlanTA.Produccion.Application.Features.OrdenesFabricacion.ListOFs;

public sealed class ListOFsQueryHandler(
    IProduccionDbContext db,
    ICurrentTenant tenant)
    : IRequestHandler<ListOFsQuery, Result<PagedResult<OrdenFabricacionListDto>>>
{
    public async Task<Result<PagedResult<OrdenFabricacionListDto>>> Handle(
        ListOFsQuery request, CancellationToken cancellationToken)
    {
        var query = db.OrdenesFabricacion
            .AsNoTracking()
            .Where(o => o.EmpresaId == tenant.EmpresaId);

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var search = request.Search.ToLowerInvariant();
            query = query.Where(o => o.CodigoOF.Value.ToLower().Contains(search));
        }

        if (request.Estado.HasValue)
        {
            query = query.Where(o => o.EstadoOF == request.Estado.Value);
        }

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderByDescending(o => o.Prioridad)
            .ThenByDescending(o => o.CreatedAt)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(o => new OrdenFabricacionListDto(
                o.Id.Value,
                o.CodigoOF.Value,
                o.ProductoId,
                o.CantidadPlanificada.Cantidad,
                o.CantidadPlanificada.UnidadMedida,
                o.EstadoOF,
                o.FechaInicio,
                o.Prioridad))
            .ToListAsync(cancellationToken);

        return Result<PagedResult<OrdenFabricacionListDto>>.Success(
            new PagedResult<OrdenFabricacionListDto>(items, totalCount, request.Page, request.PageSize));
    }
}
