using PlanTA.SharedKernel.CQRS;

namespace PlanTA.Produccion.Application.Features.OrdenesFabricacion.RegistrarProduccion;

public record RegistrarProduccionCommand(
    Guid OrdenFabricacionId,
    decimal UnidadesBuenas,
    decimal UnidadesDefectuosas = 0,
    decimal Merma = 0,
    Guid? LoteGeneradoId = null,
    string? Observaciones = null) : ICommand<Guid>;
