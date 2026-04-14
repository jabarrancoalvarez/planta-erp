using PlanTA.RRHH.Domain.Enums;
using PlanTA.SharedKernel.CQRS;

namespace PlanTA.RRHH.Application.Features.Fichajes.RegistrarFichaje;

public record RegistrarFichajeCommand(
    Guid EmpleadoId,
    TipoFichaje Tipo,
    Guid? ActivoId = null,
    Guid? OrdenFabricacionId = null,
    Guid? OrdenTrabajoId = null,
    string? Notas = null) : ICommand<Guid>;
