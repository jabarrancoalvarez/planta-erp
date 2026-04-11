using PlanTA.SharedKernel;

namespace PlanTA.Produccion.Domain.Entities;

public record ListaMaterialesId(Guid Value) : EntityId(Value);
public record LineaBOMId(Guid Value) : EntityId(Value);
public record RutaProduccionId(Guid Value) : EntityId(Value);
public record OperacionRutaId(Guid Value) : EntityId(Value);
public record OrdenFabricacionId(Guid Value) : EntityId(Value);
public record LineaConsumoOFId(Guid Value) : EntityId(Value);
public record ParteProduccionId(Guid Value) : EntityId(Value);
