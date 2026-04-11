using MediatR;
using Microsoft.EntityFrameworkCore;
using PlanTA.Compras.Application.DTOs;
using PlanTA.Compras.Application.Interfaces;
using PlanTA.Compras.Domain.Entities;
using PlanTA.Compras.Domain.Errors;
using PlanTA.SharedKernel;

namespace PlanTA.Compras.Application.Features.Recepciones.GetRecepcion;

public sealed class GetRecepcionQueryHandler(
    IComprasDbContext db,
    ICurrentTenant tenant)
    : IRequestHandler<GetRecepcionQuery, Result<RecepcionDetailDto>>
{
    public async Task<Result<RecepcionDetailDto>> Handle(GetRecepcionQuery request, CancellationToken cancellationToken)
    {
        var recepcion = await db.Recepciones
            .AsNoTracking()
            .Include(r => r.Lineas)
            .Where(r => r.Id == new RecepcionId(request.RecepcionId) && r.EmpresaId == tenant.EmpresaId)
            .FirstOrDefaultAsync(cancellationToken);

        if (recepcion is null)
            return Result<RecepcionDetailDto>.Failure(RecepcionErrors.NotFound(request.RecepcionId));

        var codigoOC = await db.OrdenesCompra
            .AsNoTracking()
            .Where(oc => oc.Id == recepcion.OrdenCompraId)
            .Select(oc => oc.Codigo)
            .FirstOrDefaultAsync(cancellationToken);

        var dto = new RecepcionDetailDto(
            recepcion.Id.Value,
            recepcion.OrdenCompraId.Value,
            codigoOC ?? string.Empty,
            recepcion.FechaRecepcion,
            recepcion.NumeroAlbaran,
            recepcion.EstadoRecepcion,
            recepcion.Observaciones,
            recepcion.CreatedAt,
            recepcion.Lineas.Select(l => new LineaRecepcionDto(
                l.Id.Value, l.LineaOrdenCompraId.Value, l.ProductoId,
                l.CantidadRecibida, l.LoteId, l.UbicacionDestinoId)).ToList());

        return Result<RecepcionDetailDto>.Success(dto);
    }
}
