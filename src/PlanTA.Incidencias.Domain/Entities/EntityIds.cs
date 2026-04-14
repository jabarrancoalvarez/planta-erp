using PlanTA.SharedKernel;

namespace PlanTA.Incidencias.Domain.Entities;

public record IncidenciaId(Guid Value) : EntityId(Value);
public record FotoIncidenciaId(Guid Value) : EntityId(Value);
public record ComentarioIncidenciaId(Guid Value) : EntityId(Value);
