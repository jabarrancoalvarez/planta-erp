using PlanTA.SharedKernel;

namespace PlanTA.Importador.Domain.Entities;

public record ImportJobId(Guid Value) : EntityId(Value);
