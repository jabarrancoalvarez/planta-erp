using PlanTA.Calidad.Application.DTOs;
using PlanTA.SharedKernel.CQRS;

namespace PlanTA.Calidad.Application.Features.Plantillas.ListPlantillas;

public record ListPlantillasQuery(
    string? Search = null,
    int Page = 1,
    int PageSize = 20) : IQuery<PagedResult<PlantillaListDto>>;
