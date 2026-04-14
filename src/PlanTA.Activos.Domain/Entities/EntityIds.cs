using PlanTA.SharedKernel;

namespace PlanTA.Activos.Domain.Entities;

public record ActivoId(Guid Value) : EntityId(Value);
public record DocumentoActivoId(Guid Value) : EntityId(Value);
public record LecturaActivoId(Guid Value) : EntityId(Value);
