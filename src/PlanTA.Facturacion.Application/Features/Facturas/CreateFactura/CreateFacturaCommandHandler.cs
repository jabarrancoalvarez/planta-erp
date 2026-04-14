using MediatR;
using Microsoft.EntityFrameworkCore;
using PlanTA.Facturacion.Application.Interfaces;
using PlanTA.Facturacion.Domain.Entities;
using PlanTA.SharedKernel;

namespace PlanTA.Facturacion.Application.Features.Facturas.CreateFactura;

public sealed class CreateFacturaCommandHandler(
    IFacturacionDbContext db,
    ICurrentTenant tenant)
    : IRequestHandler<CreateFacturaCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(CreateFacturaCommand request, CancellationToken ct)
    {
        var codigo = request.SerieCodigo.Trim().ToUpperInvariant();
        var ejercicio = (request.FechaEmision ?? DateTimeOffset.UtcNow).Year;

        var serie = await db.Series.FirstOrDefaultAsync(
            s => s.Codigo == codigo && s.Ejercicio == ejercicio && s.EmpresaId == tenant.EmpresaId, ct);

        if (serie is null)
        {
            serie = SerieFactura.Crear(codigo, $"Serie {codigo} {ejercicio}", ejercicio, tenant.EmpresaId);
            db.Series.Add(serie);
        }

        var numero = serie.SiguienteNumero();

        var factura = Factura.Crear(
            codigo, numero, ejercicio, request.ClienteId, request.ClienteNombre, tenant.EmpresaId,
            request.Tipo, request.ClienteNIF, request.ClienteDireccion,
            request.FechaEmision, request.FechaVencimiento, request.Observaciones);

        foreach (var linea in request.Lineas)
        {
            factura.AgregarLinea(
                linea.Descripcion, linea.Cantidad, linea.PrecioUnitario,
                linea.IvaPct, linea.DescuentoPct, linea.ProductoId);
        }

        db.Facturas.Add(factura);
        await db.SaveChangesAsync(ct);
        return Result<Guid>.Success(factura.Id.Value);
    }
}
