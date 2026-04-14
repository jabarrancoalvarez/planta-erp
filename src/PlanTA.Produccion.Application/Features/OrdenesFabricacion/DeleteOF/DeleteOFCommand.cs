using PlanTA.SharedKernel.CQRS;

namespace PlanTA.Produccion.Application.Features.OrdenesFabricacion.DeleteOF;

public record DeleteOFCommand(Guid OrdenFabricacionId) : ICommand<Guid>;
