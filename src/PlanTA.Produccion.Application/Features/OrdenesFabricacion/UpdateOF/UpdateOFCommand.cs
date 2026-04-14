using PlanTA.SharedKernel.CQRS;

namespace PlanTA.Produccion.Application.Features.OrdenesFabricacion.UpdateOF;

public record UpdateOFCommand(
    Guid OrdenFabricacionId,
    int Prioridad,
    string? Observaciones) : ICommand<bool>;
