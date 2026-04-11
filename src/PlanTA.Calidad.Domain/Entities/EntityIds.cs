using PlanTA.SharedKernel;

namespace PlanTA.Calidad.Domain.Entities;

public record PlantillaInspeccionId(Guid Value) : EntityId(Value);
public record CriterioInspeccionId(Guid Value) : EntityId(Value);
public record InspeccionId(Guid Value) : EntityId(Value);
public record ResultadoCriterioId(Guid Value) : EntityId(Value);
public record NoConformidadId(Guid Value) : EntityId(Value);
public record AccionCorrectivaId(Guid Value) : EntityId(Value);
