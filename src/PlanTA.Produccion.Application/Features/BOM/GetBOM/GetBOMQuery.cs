using PlanTA.Produccion.Application.DTOs;
using PlanTA.SharedKernel.CQRS;

namespace PlanTA.Produccion.Application.Features.BOM.GetBOM;

public record GetBOMQuery(Guid ListaMaterialesId) : IQuery<ListaMaterialesDetailDto>;
