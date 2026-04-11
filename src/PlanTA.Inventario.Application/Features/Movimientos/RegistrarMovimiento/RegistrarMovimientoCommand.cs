using PlanTA.Inventario.Domain.Enums;
using PlanTA.SharedKernel.CQRS;

namespace PlanTA.Inventario.Application.Features.Movimientos.RegistrarMovimiento;

public record RegistrarMovimientoCommand(
    Guid ProductoId,
    Guid AlmacenId,
    TipoMovimiento Tipo,
    decimal Cantidad,
    Guid? UbicacionId = null,
    Guid? LoteId = null,
    string? Referencia = null,
    string? Notas = null) : ICommand<Guid>;
