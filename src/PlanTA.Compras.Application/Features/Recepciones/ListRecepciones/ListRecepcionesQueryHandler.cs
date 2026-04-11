using MediatR;
using Microsoft.EntityFrameworkCore;
using PlanTA.Compras.Application.DTOs;
using PlanTA.Compras.Application.Interfaces;
using PlanTA.Compras.Domain.Entities;
using PlanTA.SharedKernel;

namespace PlanTA.Compras.Application.Features.Recepciones.ListRecepciones;

public sealed class ListRecepcionesQueryHandler(
    IComprasDbContext db,
    ICurrentTenant tenant)
    : IRequestHandler<ListRecepcionesQuery, Result<PagedResult<RecepcionListDto>>>
{
    public async Task<Result<PagedResult<RecepcionListDto>>> Handle(
        ListRecepcionesQuery request, CancellationToken cancellationToken)
    {
        var query = db.Recepciones
            .AsNoTracking()
            .Where(r => r.EmpresaId == tenant.EmpresaId);

        if (request.OrdenCompraId.HasValue)
            query = query.Where(r => r.OrdenCompraId == new OrdenCompraId(request.OrdenCompraId.Value));

        if (request.Estado.HasValue)
            query = query.Where(r => r.EstadoRecepcion == request.Estado.Value);

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderByDescending(r => r.FechaRecepcion)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(r => new RecepcionListDto(
                r.Id.Value,
                r.OrdenCompraId.Value,
                db.OrdenesCompra.Where(oc => oc.Id == r.OrdenCompraId).Select(oc => oc.Codigo).FirstOrDefault() ?? string.Empty,
                r.FechaRecepcion,
                r.NumeroAlbaran,
                r.EstadoRecepcion))
            .ToListAsync(cancellationToken);

        return Result<PagedResult<RecepcionListDto>>.Success(
            new PagedResult<RecepcionListDto>(items, totalCount, request.Page, request.PageSize));
    }
}
