using PlanTA.Costes.Domain.Enums;
using PlanTA.SharedKernel.CQRS;

namespace PlanTA.Costes.Application.Features.Imputaciones.CreateImputacion;

public record CreateImputacionCommand(
    TipoCoste Tipo,
    OrigenImputacion Origen,
    decimal Cantidad,
    decimal PrecioUnitario,
    Guid? OrdenFabricacionId = null,
    Guid? OrdenTrabajoId = null,
    Guid? ProductoId = null,
    string? Concepto = null,
    DateTimeOffset? Fecha = null) : ICommand<Guid>;
