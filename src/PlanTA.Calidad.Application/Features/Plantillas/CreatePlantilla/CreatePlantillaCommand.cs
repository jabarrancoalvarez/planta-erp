using PlanTA.Calidad.Domain.Enums;
using PlanTA.SharedKernel.CQRS;

namespace PlanTA.Calidad.Application.Features.Plantillas.CreatePlantilla;

public record CreatePlantillaCommand(
    string Nombre,
    OrigenInspeccion OrigenInspeccion,
    string? Descripcion = null) : ICommand<Guid>;
