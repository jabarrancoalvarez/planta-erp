using PlanTA.SharedKernel.CQRS;

namespace PlanTA.Inventario.Application.Features.Alertas.CreateAlerta;

public record CreateAlertaCommand(
    Guid ProductoId,
    decimal StockMinimo,
    decimal StockMaximo,
    Guid? AlmacenId = null,
    decimal PuntoReorden = 0,
    decimal CantidadReorden = 0,
    bool AutoReorden = false) : ICommand<Guid>;
