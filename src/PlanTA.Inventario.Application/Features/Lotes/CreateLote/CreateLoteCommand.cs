using PlanTA.SharedKernel.CQRS;

namespace PlanTA.Inventario.Application.Features.Lotes.CreateLote;

public record CreateLoteCommand(
    Guid ProductoId,
    decimal Cantidad,
    string? CodigoLote = null,
    DateTimeOffset? FechaCaducidad = null,
    string? Origen = null,
    string? Notas = null) : ICommand<Guid>;
