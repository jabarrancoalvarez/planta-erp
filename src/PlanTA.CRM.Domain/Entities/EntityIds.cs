using PlanTA.SharedKernel;

namespace PlanTA.CRM.Domain.Entities;

public record LeadId(Guid Value) : EntityId(Value);
public record OportunidadId(Guid Value) : EntityId(Value);
public record ActividadCrmId(Guid Value) : EntityId(Value);
