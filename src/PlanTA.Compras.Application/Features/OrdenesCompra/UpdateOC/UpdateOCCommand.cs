using PlanTA.SharedKernel.CQRS;

namespace PlanTA.Compras.Application.Features.OrdenesCompra.UpdateOC;

public record UpdateOCCommand(
    Guid OrdenCompraId,
    DateTimeOffset? FechaEntregaEstimada,
    string? Observaciones) : ICommand<bool>;
