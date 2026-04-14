using PlanTA.Mantenimiento.Domain.Entities;
using PlanTA.SharedKernel.CQRS;

namespace PlanTA.Mantenimiento.Domain.Events;

public sealed record OrdenTrabajoCreadaEvent(OrdenTrabajoId Id, Guid ActivoId, int Tipo) : IDomainEvent;
public sealed record OrdenTrabajoCompletadaEvent(OrdenTrabajoId Id, Guid ActivoId, decimal HorasInvertidas) : IDomainEvent;
