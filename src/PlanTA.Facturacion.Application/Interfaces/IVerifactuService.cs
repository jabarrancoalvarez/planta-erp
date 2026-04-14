using PlanTA.Facturacion.Domain.Entities;

namespace PlanTA.Facturacion.Application.Interfaces;

public interface IVerifactuService
{
    Task<VerifactuResultado> EnviarAsync(Factura factura, CancellationToken ct = default);
}

public record VerifactuResultado(bool Exito, string Csv, string Respuesta, string? Error = null);
