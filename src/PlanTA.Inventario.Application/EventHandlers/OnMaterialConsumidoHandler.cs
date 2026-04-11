using MediatR;
using Microsoft.Extensions.Logging;
using PlanTA.SharedKernel.IntegrationEvents;

namespace PlanTA.Inventario.Application.EventHandlers;

/// <summary>
/// Produccion -> Inventario: cuando se consume material en una OF,
/// se registra un MovimientoStock de salida.
/// </summary>
public sealed class OnMaterialConsumidoHandler(
    ILogger<OnMaterialConsumidoHandler> logger)
    : INotificationHandler<MaterialConsumidoIntegrationEvent>
{
    public Task Handle(MaterialConsumidoIntegrationEvent notification, CancellationToken cancellationToken)
    {
        logger.LogInformation(
            "[Inventario] Material consumido en OF {OFId}: " +
            "Producto {ProductoId}, Cantidad {Cantidad}, Lote {LoteId}. " +
            "Pendiente: registrar MovimientoStock salida",
            notification.OrdenFabricacionId,
            notification.ProductoId,
            notification.Cantidad,
            notification.LoteId);

        // TODO Phase 7: implementar logica real
        // 1. Buscar stock del producto (y lote si aplica)
        // 2. Registrar MovimientoStock de tipo Salida/Consumo
        // 3. Verificar si stock baja del minimo -> disparar StockBajoMinimoIntegrationEvent

        return Task.CompletedTask;
    }
}
