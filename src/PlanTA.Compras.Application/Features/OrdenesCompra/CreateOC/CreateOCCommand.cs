using PlanTA.SharedKernel.CQRS;

namespace PlanTA.Compras.Application.Features.OrdenesCompra.CreateOC;

public record CreateOCCommand(
    string Codigo,
    Guid ProveedorId,
    DateTimeOffset? FechaEntregaEstimada = null,
    string? Observaciones = null) : ICommand<Guid>;
