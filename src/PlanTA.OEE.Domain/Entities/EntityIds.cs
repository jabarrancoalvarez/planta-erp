using PlanTA.SharedKernel;

namespace PlanTA.OEE.Domain.Entities;

public record RegistroOEEId(Guid Value) : EntityId(Value);
