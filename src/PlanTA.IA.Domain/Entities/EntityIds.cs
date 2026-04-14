using PlanTA.SharedKernel;

namespace PlanTA.IA.Domain.Entities;

public record ConversacionIAId(Guid Value) : EntityId(Value);
public record MensajeIAId(Guid Value) : EntityId(Value);
