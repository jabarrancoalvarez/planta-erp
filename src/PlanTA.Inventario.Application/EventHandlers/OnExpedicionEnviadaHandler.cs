using MediatR;
using Microsoft.Extensions.Logging;
using PlanTA.SharedKernel.IntegrationEvents;

namespace PlanTA.Inventario.Application.EventHandlers;

/// <summary>
/// Ventas -> Inventario: cuando una expedicion es enviada,
/// se registra MovimientoStock de salida por cada linea.
/// </summary>
public sealed class OnExpedicionEnviadaHandler(
    ILogger<OnExpedicionEnviadaHandler> logger)
    : INotificationHandler<ExpedicionEnviadaIntegrationEvent>
{
    public Task Handle(ExpedicionEnviadaIntegrationEvent notification, CancellationToken cancellationToken)
    {
        logger.LogInformation(
            "[Inventario] Expedicion {ExpedicionId} enviada con {LineaCount} lineas. " +
            "Pendiente: registrar MovimientoStock salida por cada linea",
            notification.ExpedicionId,
            notification.Lineas.Count);

        // TODO Phase 7: implementar logica real
        // Para cada linea:
        // 1. Registrar MovimientoStock de tipo Salida/Expedicion
        // 2. Descontar stock del lote de origen si aplica
        // 3. Verificar si stock baja del minimo

        return Task.CompletedTask;
    }
}
