using PlanTA.Compras.Domain.Enums;
using PlanTA.SharedKernel.CQRS;

namespace PlanTA.Compras.Application.Features.Recepciones.CambiarEstadoRecepcion;

public record CambiarEstadoRecepcionCommand(
    Guid RecepcionId,
    EstadoRecepcion EstadoDestino,
    string? Motivo = null) : ICommand<bool>;
