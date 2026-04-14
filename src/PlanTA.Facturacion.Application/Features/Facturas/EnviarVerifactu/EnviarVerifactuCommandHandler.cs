using MediatR;
using Microsoft.EntityFrameworkCore;
using PlanTA.Facturacion.Application.Interfaces;
using PlanTA.Facturacion.Domain.Entities;
using PlanTA.Facturacion.Domain.Errors;
using PlanTA.SharedKernel;

namespace PlanTA.Facturacion.Application.Features.Facturas.EnviarVerifactu;

public sealed class EnviarVerifactuCommandHandler(
    IFacturacionDbContext db,
    IVerifactuService verifactu,
    ICurrentTenant tenant)
    : IRequestHandler<EnviarVerifactuCommand, Result<string>>
{
    public async Task<Result<string>> Handle(EnviarVerifactuCommand request, CancellationToken ct)
    {
        var id = new FacturaId(request.Id);
        var factura = await db.Facturas
            .FirstOrDefaultAsync(f => f.Id == id && f.EmpresaId == tenant.EmpresaId, ct);

        if (factura is null)
            return Result<string>.Failure(FacturaErrors.NotFound(request.Id));

        var res = await verifactu.EnviarAsync(factura, ct);
        if (!res.Exito)
            return Result<string>.Failure("Verifactu.Error", res.Error ?? "Error desconocido");

        factura.MarcarEnviadaVerifactu(res.Csv, res.Respuesta);
        await db.SaveChangesAsync(ct);

        return Result<string>.Success(res.Csv);
    }
}
