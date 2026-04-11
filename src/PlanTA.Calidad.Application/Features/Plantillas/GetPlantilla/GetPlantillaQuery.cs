using PlanTA.Calidad.Application.DTOs;
using PlanTA.SharedKernel.CQRS;

namespace PlanTA.Calidad.Application.Features.Plantillas.GetPlantilla;

public record GetPlantillaQuery(Guid PlantillaId) : IQuery<PlantillaDetailDto>;
