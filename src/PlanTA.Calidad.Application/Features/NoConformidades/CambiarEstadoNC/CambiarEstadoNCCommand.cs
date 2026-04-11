using PlanTA.Calidad.Domain.Enums;
using PlanTA.SharedKernel.CQRS;

namespace PlanTA.Calidad.Application.Features.NoConformidades.CambiarEstadoNC;

public record CambiarEstadoNCCommand(
    Guid NoConformidadId,
    EstadoNoConformidad EstadoDestino,
    string? CausaRaiz = null) : ICommand<bool>;
