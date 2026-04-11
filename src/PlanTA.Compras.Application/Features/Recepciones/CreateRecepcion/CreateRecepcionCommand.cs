using PlanTA.SharedKernel.CQRS;

namespace PlanTA.Compras.Application.Features.Recepciones.CreateRecepcion;

public record CreateRecepcionCommand(
    Guid OrdenCompraId,
    string? NumeroAlbaran = null,
    string? Observaciones = null,
    List<LineaRecepcionRequest>? Lineas = null) : ICommand<Guid>;

public record LineaRecepcionRequest(
    Guid LineaOrdenCompraId,
    Guid ProductoId,
    decimal CantidadRecibida,
    Guid? LoteId = null,
    Guid? UbicacionDestinoId = null);
