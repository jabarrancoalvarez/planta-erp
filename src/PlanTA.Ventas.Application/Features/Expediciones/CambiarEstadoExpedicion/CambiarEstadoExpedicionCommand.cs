using PlanTA.Ventas.Domain.Enums;
using PlanTA.SharedKernel.CQRS;

namespace PlanTA.Ventas.Application.Features.Expediciones.CambiarEstadoExpedicion;

public record CambiarEstadoExpedicionCommand(
    Guid ExpedicionId,
    EstadoExpedicion EstadoDestino) : ICommand<bool>;
