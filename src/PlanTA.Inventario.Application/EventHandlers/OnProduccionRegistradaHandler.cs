using MediatR;
using Microsoft.Extensions.Logging;
using PlanTA.SharedKernel.IntegrationEvents;

namespace PlanTA.Inventario.Application.EventHandlers;

/// <summary>
/// Produccion -> Inventario: cuando se registra produccion terminada,
/// se crea un Lote del producto fabricado y MovimientoStock de entrada.
/// </summary>
public sealed class OnProduccionRegistradaHandler(
    ILogger<OnProduccionRegistradaHandler> logger)
    : INotificationHandler<ProduccionRegistradaIntegrationEvent>
{
    public Task Handle(ProduccionRegistradaIntegrationEvent notification, CancellationToken cancellationToken)
    {
        logger.LogInformation(
            "[Inventario] Produccion registrada en OF {OFId}: " +
            "Cantidad {Cantidad}, LoteGenerado {LoteId}. " +
            "Pendiente: crear Lote + MovimientoStock entrada",
            notification.OrdenFabricacionId,
            notification.Cantidad,
            notification.LoteGeneradoId);

        // TODO Phase 7: implementar logica real
        // 1. Crear Lote para el producto fabricado
        // 2. Registrar MovimientoStock de tipo Entrada/Produccion

        return Task.CompletedTask;
    }
}
