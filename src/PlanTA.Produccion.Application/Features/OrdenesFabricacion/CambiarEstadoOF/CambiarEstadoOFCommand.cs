using PlanTA.Produccion.Domain.Enums;
using PlanTA.SharedKernel.CQRS;

namespace PlanTA.Produccion.Application.Features.OrdenesFabricacion.CambiarEstadoOF;

public record CambiarEstadoOFCommand(
    Guid OrdenFabricacionId,
    EstadoOF EstadoDestino,
    string? Motivo = null) : ICommand<bool>;
