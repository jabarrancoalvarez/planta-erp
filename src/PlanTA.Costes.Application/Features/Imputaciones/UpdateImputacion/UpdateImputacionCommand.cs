using PlanTA.SharedKernel.CQRS;

namespace PlanTA.Costes.Application.Features.Imputaciones.UpdateImputacion;

public record UpdateImputacionCommand(
    Guid ImputacionId,
    decimal Cantidad,
    decimal PrecioUnitario,
    string? Concepto,
    DateTimeOffset Fecha) : ICommand<bool>;
