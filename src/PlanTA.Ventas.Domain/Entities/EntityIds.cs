using PlanTA.SharedKernel;

namespace PlanTA.Ventas.Domain.Entities;

public record ClienteId(Guid Value) : EntityId(Value);
public record ContactoClienteId(Guid Value) : EntityId(Value);
public record PedidoId(Guid Value) : EntityId(Value);
public record LineaPedidoId(Guid Value) : EntityId(Value);
public record ExpedicionId(Guid Value) : EntityId(Value);
public record LineaExpedicionId(Guid Value) : EntityId(Value);
