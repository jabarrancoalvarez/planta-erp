using PlanTA.Facturacion.Application.Interfaces;
using PlanTA.Facturacion.Domain.Entities;

namespace PlanTA.Facturacion.Infrastructure.Services;

public class VerifactuMockService : IVerifactuService
{
    public Task<VerifactuResultado> EnviarAsync(Factura factura, CancellationToken ct = default)
    {
        var csv = GenerarCsvMock(factura);
        var respuesta = $"{{\"estado\":\"Aceptada\",\"csv\":\"{csv}\",\"timestamp\":\"{DateTimeOffset.UtcNow:o}\"}}";
        return Task.FromResult(new VerifactuResultado(true, csv, respuesta));
    }

    private static string GenerarCsvMock(Factura factura)
    {
        var seed = $"{factura.NumeroCompleto}{factura.Total}{factura.FechaEmision:yyyyMMdd}";
        var hash = Math.Abs(seed.GetHashCode()).ToString("X8");
        return $"A{hash}B{DateTime.UtcNow:yyMMdd}";
    }
}
