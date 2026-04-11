using MediatR;
using Microsoft.Extensions.Logging;
using PlanTA.SharedKernel.IntegrationEvents;

namespace PlanTA.Inventario.Application.EventHandlers;

/// <summary>
/// Compras -> Inventario: cuando una recepcion es aceptada,
/// se crea un Lote y un MovimientoStock de entrada.
/// </summary>
public sealed class OnRecepcionAceptadaHandler(
    ILogger<OnRecepcionAceptadaHandler> logger)
    : INotificationHandler<RecepcionAceptadaIntegrationEvent>
{
    public Task Handle(RecepcionAceptadaIntegrationEvent notification, CancellationToken cancellationToken)
    {
        logger.LogInformation(
            "[Inventario] Recepcion {RecepcionId} aceptada. " +
            "Pendiente: crear Lote + MovimientoStock entrada",
            notification.RecepcionId);

        // TODO Phase 7: implementar logica real
        // 1. Obtener lineas de la recepcion (via query a Compras o datos en el evento)
        // 2. Para cada linea, crear o actualizar Lote
        // 3. Registrar MovimientoStock de tipo Entrada

        return Task.CompletedTask;
    }
}
