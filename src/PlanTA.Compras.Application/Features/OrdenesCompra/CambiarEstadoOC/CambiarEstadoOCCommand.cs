using PlanTA.Compras.Domain.Enums;
using PlanTA.SharedKernel.CQRS;

namespace PlanTA.Compras.Application.Features.OrdenesCompra.CambiarEstadoOC;

public record CambiarEstadoOCCommand(
    Guid OrdenCompraId,
    EstadoOC EstadoDestino,
    string? Motivo = null) : ICommand<bool>;
