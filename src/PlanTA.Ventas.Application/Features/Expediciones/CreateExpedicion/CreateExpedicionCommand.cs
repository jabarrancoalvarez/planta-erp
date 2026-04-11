using PlanTA.SharedKernel.CQRS;

namespace PlanTA.Ventas.Application.Features.Expediciones.CreateExpedicion;

public record CreateExpedicionCommand(
    Guid PedidoId,
    string? NumeroSeguimiento = null,
    string? Transportista = null,
    string? Observaciones = null,
    List<LineaExpedicionRequest>? Lineas = null) : ICommand<Guid>;

public record LineaExpedicionRequest(
    Guid LineaPedidoId,
    Guid ProductoId,
    decimal Cantidad,
    Guid? LoteOrigenId = null);
