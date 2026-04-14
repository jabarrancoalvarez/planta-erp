using PlanTA.SharedKernel;

namespace PlanTA.RRHH.Domain.Entities;

public record EmpleadoId(Guid Value) : EntityId(Value);
public record TurnoId(Guid Value) : EntityId(Value);
public record FichajeId(Guid Value) : EntityId(Value);
public record AusenciaId(Guid Value) : EntityId(Value);
