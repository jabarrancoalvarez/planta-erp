using PlanTA.Facturacion.Domain.Enums;
using PlanTA.SharedKernel.CQRS;

namespace PlanTA.Facturacion.Application.Features.Facturas.CreateFactura;

public record LineaFacturaInput(
    string Descripcion, decimal Cantidad, decimal PrecioUnitario,
    decimal IvaPct, decimal DescuentoPct = 0, Guid? ProductoId = null);

public record CreateFacturaCommand(
    string SerieCodigo,
    Guid ClienteId,
    string ClienteNombre,
    List<LineaFacturaInput> Lineas,
    TipoFactura Tipo = TipoFactura.Ordinaria,
    string? ClienteNIF = null,
    string? ClienteDireccion = null,
    DateTimeOffset? FechaEmision = null,
    DateTimeOffset? FechaVencimiento = null,
    string? Observaciones = null) : ICommand<Guid>;
