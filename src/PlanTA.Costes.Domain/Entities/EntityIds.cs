using PlanTA.SharedKernel;

namespace PlanTA.Costes.Domain.Entities;

public record ImputacionCosteId(Guid Value) : EntityId(Value);
public record CosteEstandarId(Guid Value) : EntityId(Value);
