using PlanTA.Incidencias.Domain.Entities;
using PlanTA.SharedKernel.CQRS;

namespace PlanTA.Incidencias.Domain.Events;

public sealed record IncidenciaAbiertaEvent(IncidenciaId Id, Guid? ActivoId, int Severidad, int Tipo) : IDomainEvent;
public sealed record IncidenciaCerradaEvent(IncidenciaId Id, Guid? OrdenTrabajoId) : IDomainEvent;
