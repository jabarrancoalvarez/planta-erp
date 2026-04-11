using PlanTA.SharedKernel.CQRS;

namespace PlanTA.SharedKernel.IntegrationEvents;

/// <summary>
/// Ventas -> Inventario: expedicion enviada, registrar salida de stock por cada linea.
/// </summary>
public sealed record ExpedicionEnviadaIntegrationEvent(
    Guid ExpedicionId,
    List<LineaExpedicionIntegrationInfo> Lineas) : IDomainEvent;

public sealed record LineaExpedicionIntegrationInfo(
    Guid ProductoId,
    decimal Cantidad,
    Guid? LoteOrigenId);
