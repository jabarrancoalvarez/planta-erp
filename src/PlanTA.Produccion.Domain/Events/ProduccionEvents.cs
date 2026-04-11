using PlanTA.Produccion.Domain.Entities;
using PlanTA.SharedKernel.CQRS;

namespace PlanTA.Produccion.Domain.Events;

public sealed record OFCreadaEvent(
    OrdenFabricacionId OrdenFabricacionId,
    Guid ProductoId,
    decimal Cantidad) : IDomainEvent;

public sealed record OFIniciadaEvent(
    OrdenFabricacionId OrdenFabricacionId) : IDomainEvent;

public sealed record OFCompletadaEvent(
    OrdenFabricacionId OrdenFabricacionId,
    decimal UnidadesBuenas,
    decimal Defectuosas) : IDomainEvent;

public sealed record OFCanceladaEvent(
    OrdenFabricacionId OrdenFabricacionId,
    string Motivo) : IDomainEvent;

public sealed record MaterialConsumidoEvent(
    OrdenFabricacionId OrdenFabricacionId,
    Guid ProductoId,
    Guid? LoteId,
    decimal Cantidad) : IDomainEvent;

public sealed record ProduccionRegistradaEvent(
    OrdenFabricacionId OrdenFabricacionId,
    decimal Cantidad,
    Guid? LoteGeneradoId) : IDomainEvent;
