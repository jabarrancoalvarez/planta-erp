using PlanTA.SharedKernel.CQRS;

namespace PlanTA.Calidad.Application.Features.Inspecciones.RegistrarResultado;

public record RegistrarResultadoCommand(
    Guid InspeccionId,
    Guid CriterioInspeccionId,
    string? ValorMedido,
    bool Cumple,
    string? Observaciones = null) : ICommand<bool>;
