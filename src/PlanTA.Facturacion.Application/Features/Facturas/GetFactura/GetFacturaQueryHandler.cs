using MediatR;
using Microsoft.EntityFrameworkCore;
using PlanTA.Facturacion.Application.DTOs;
using PlanTA.Facturacion.Application.Interfaces;
using PlanTA.Facturacion.Domain.Entities;
using PlanTA.Facturacion.Domain.Errors;
using PlanTA.SharedKernel;

namespace PlanTA.Facturacion.Application.Features.Facturas.GetFactura;

public sealed class GetFacturaQueryHandler(
    IFacturacionDbContext db,
    ICurrentTenant tenant)
    : IRequestHandler<GetFacturaQuery, Result<FacturaDto>>
{
    public async Task<Result<FacturaDto>> Handle(GetFacturaQuery request, CancellationToken ct)
    {
        var id = new FacturaId(request.Id);
        var f = await db.Facturas.AsNoTracking()
            .Include(x => x.Lineas)
            .FirstOrDefaultAsync(x => x.Id == id && x.EmpresaId == tenant.EmpresaId, ct);

        if (f is null)
            return Result<FacturaDto>.Failure(FacturaErrors.NotFound(request.Id));

        var dto = new FacturaDto(
            f.Id.Value, f.NumeroCompleto, f.SerieCodigo, f.Numero, f.Ejercicio,
            f.Tipo, f.Estado, f.ClienteId, f.ClienteNombre, f.ClienteNIF, f.ClienteDireccion,
            f.FechaEmision, f.FechaVencimiento, f.BaseImponible, f.TotalIva, f.Total, f.Observaciones,
            f.HashPrevio, f.HashActual, f.CodigoQrVerifactu, f.EstadoVerifactu, f.VerifactuCsv,
            f.Lineas.OrderBy(l => l.NumeroLinea).Select(l => new LineaFacturaDto(
                l.Id.Value, l.NumeroLinea, l.Descripcion,
                l.Cantidad, l.PrecioUnitario, l.DescuentoPct, l.IvaPct,
                l.BaseImponible, l.Iva, l.Total, l.ProductoId)).ToList());

        return Result<FacturaDto>.Success(dto);
    }
}
