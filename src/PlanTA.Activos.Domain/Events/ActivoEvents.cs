using PlanTA.Activos.Domain.Entities;
using PlanTA.SharedKernel.CQRS;

namespace PlanTA.Activos.Domain.Events;

public sealed record ActivoCreadoEvent(ActivoId ActivoId, string Codigo) : IDomainEvent;
public sealed record EstadoActivoCambiadoEvent(ActivoId ActivoId, int EstadoAnterior, int EstadoNuevo) : IDomainEvent;
