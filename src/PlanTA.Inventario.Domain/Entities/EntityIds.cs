using PlanTA.SharedKernel;

namespace PlanTA.Inventario.Domain.Entities;

public record ProductoId(Guid Value) : EntityId(Value);
public record CategoriaProductoId(Guid Value) : EntityId(Value);
public record AlmacenId(Guid Value) : EntityId(Value);
public record UbicacionId(Guid Value) : EntityId(Value);
public record LoteId(Guid Value) : EntityId(Value);
public record MovimientoStockId(Guid Value) : EntityId(Value);
public record AlertaStockId(Guid Value) : EntityId(Value);
