using MediatR;
using Microsoft.EntityFrameworkCore;
using PlanTA.Facturacion.Application.Interfaces;
using PlanTA.Facturacion.Domain.Entities;
using PlanTA.Facturacion.Domain.Errors;
using PlanTA.SharedKernel;

namespace PlanTA.Facturacion.Application.Features.Facturas.EmitirFactura;

public sealed class EmitirFacturaCommandHandler(
    IFacturacionDbContext db,
    ICurrentTenant tenant)
    : IRequestHandler<EmitirFacturaCommand, Result<bool>>
{
    public async Task<Result<bool>> Handle(EmitirFacturaCommand request, CancellationToken ct)
    {
        var id = new FacturaId(request.Id);
        var factura = await db.Facturas
            .Include(f => f.Lineas)
            .FirstOrDefaultAsync(f => f.Id == id && f.EmpresaId == tenant.EmpresaId, ct);

        if (factura is null)
            return Result<bool>.Failure(FacturaErrors.NotFound(request.Id));

        // Buscar hash de la última factura emitida (cadena de hashes Verifactu)
        var hashPrevio = await db.Facturas
            .Where(f => f.EmpresaId == tenant.EmpresaId && f.HashActual != null)
            .OrderByDescending(f => f.FechaEmision)
            .Select(f => f.HashActual)
            .FirstOrDefaultAsync(ct);

        var result = factura.Emitir(hashPrevio);
        if (!result.IsSuccess) return result;

        await db.SaveChangesAsync(ct);
        return Result<bool>.Success(true);
    }
}
