using MediatR;
using Microsoft.Extensions.Logging;
using PlanTA.SharedKernel.IntegrationEvents;

namespace PlanTA.Inventario.Application.EventHandlers;

/// <summary>
/// Calidad -> Inventario: cuando una inspeccion es rechazada,
/// se bloquea el lote afectado.
/// </summary>
public sealed class OnInspeccionRechazadaHandler(
    ILogger<OnInspeccionRechazadaHandler> logger)
    : INotificationHandler<InspeccionRechazadaIntegrationEvent>
{
    public Task Handle(InspeccionRechazadaIntegrationEvent notification, CancellationToken cancellationToken)
    {
        logger.LogInformation(
            "[Inventario] Inspeccion {InspeccionId} rechazada para Lote {LoteId}. " +
            "Pendiente: bloquear lote",
            notification.InspeccionId,
            notification.LoteId);

        // TODO Phase 7: implementar logica real
        // 1. Buscar Lote por Id
        // 2. Cambiar estado a Bloqueado
        // 3. Registrar motivo del bloqueo

        return Task.CompletedTask;
    }
}
