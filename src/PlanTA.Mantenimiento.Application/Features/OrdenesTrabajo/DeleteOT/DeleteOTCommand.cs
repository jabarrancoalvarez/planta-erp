using PlanTA.SharedKernel.CQRS;

namespace PlanTA.Mantenimiento.Application.Features.OrdenesTrabajo.DeleteOT;

public record DeleteOTCommand(Guid OrdenTrabajoId) : ICommand<Guid>;
