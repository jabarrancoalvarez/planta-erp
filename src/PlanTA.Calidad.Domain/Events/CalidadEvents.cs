using PlanTA.Calidad.Domain.Entities;
using PlanTA.Calidad.Domain.Enums;
using PlanTA.SharedKernel.CQRS;

namespace PlanTA.Calidad.Domain.Events;

public sealed record InspeccionCompletadaEvent(
    InspeccionId InspeccionId,
    ResultadoInspeccion Resultado) : IDomainEvent;

public sealed record InspeccionRechazadaEvent(
    InspeccionId InspeccionId,
    Guid? LoteId) : IDomainEvent;

public sealed record NoConformidadAbiertaEvent(
    NoConformidadId NoConformidadId,
    OrigenInspeccion Origen) : IDomainEvent;

public sealed record NoConformidadResueltaEvent(
    NoConformidadId NoConformidadId) : IDomainEvent;
