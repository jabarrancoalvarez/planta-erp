using MediatR;
using Microsoft.EntityFrameworkCore;
using PlanTA.Inventario.Application.DTOs;
using PlanTA.Inventario.Application.Interfaces;
using PlanTA.Inventario.Domain.Entities;
using PlanTA.SharedKernel;

namespace PlanTA.Inventario.Application.Features.Alertas.ListAlertas;

public sealed class ListAlertasQueryHandler(
    IInventarioDbContext db,
    ICurrentTenant tenant)
    : IRequestHandler<ListAlertasQuery, Result<List<AlertaStockDto>>>
{
    public async Task<Result<List<AlertaStockDto>>> Handle(
        ListAlertasQuery request, CancellationToken cancellationToken)
    {
        var query = db.Alertas
            .AsNoTracking()
            .Where(a => a.EmpresaId == tenant.EmpresaId && a.Activa);

        if (request.ProductoId.HasValue)
            query = query.Where(a => a.ProductoId == new ProductoId(request.ProductoId.Value));

        var alertas = await query
            .Select(a => new AlertaStockDto(
                a.Id.Value, a.ProductoId.Value,
                a.AlmacenId != null ? a.AlmacenId.Value : null,
                a.StockMinimo, a.StockMaximo,
                a.PuntoReorden, a.CantidadReorden,
                a.AutoReorden, a.Activa))
            .ToListAsync(cancellationToken);

        return Result<List<AlertaStockDto>>.Success(alertas);
    }
}
