using PlanTA.SharedKernel.CQRS;

namespace PlanTA.OEE.Application.Features.Registros.RegistrarOEE;

public record RegistrarOEECommand(
    Guid ActivoId,
    DateTimeOffset Fecha,
    int MinutosPlanificados,
    int MinutosFuncionamiento,
    int PiezasTotales,
    int PiezasBuenas,
    decimal TiempoCicloTeoricoSeg,
    Guid? TurnoId = null,
    Guid? OrdenFabricacionId = null,
    string? Notas = null) : ICommand<Guid>;
