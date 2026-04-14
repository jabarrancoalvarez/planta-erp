using PlanTA.SharedKernel;

namespace PlanTA.Mantenimiento.Domain.Entities;

public record OrdenTrabajoId(Guid Value) : EntityId(Value);
public record PlanMantenimientoId(Guid Value) : EntityId(Value);
public record TareaOTId(Guid Value) : EntityId(Value);
public record RepuestoOTId(Guid Value) : EntityId(Value);
