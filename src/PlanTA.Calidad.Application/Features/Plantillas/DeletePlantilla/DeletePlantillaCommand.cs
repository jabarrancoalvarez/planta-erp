using PlanTA.SharedKernel.CQRS;

namespace PlanTA.Calidad.Application.Features.Plantillas.DeletePlantilla;

public record DeletePlantillaCommand(Guid PlantillaId) : ICommand<Guid>;
