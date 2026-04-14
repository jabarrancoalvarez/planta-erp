using PlanTA.SharedKernel;

namespace PlanTA.Facturacion.Domain.Entities;

public record FacturaId(Guid Value) : EntityId(Value);
public record LineaFacturaId(Guid Value) : EntityId(Value);
public record SerieFacturaId(Guid Value) : EntityId(Value);
