using PlanTA.SharedKernel;

namespace PlanTA.Compras.Domain.Entities;

public record ProveedorId(Guid Value) : EntityId(Value);
public record ContactoProveedorId(Guid Value) : EntityId(Value);
public record OrdenCompraId(Guid Value) : EntityId(Value);
public record LineaOrdenCompraId(Guid Value) : EntityId(Value);
public record RecepcionId(Guid Value) : EntityId(Value);
public record LineaRecepcionId(Guid Value) : EntityId(Value);
