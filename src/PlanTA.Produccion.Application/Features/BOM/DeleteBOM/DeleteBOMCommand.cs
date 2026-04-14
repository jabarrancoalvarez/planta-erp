using PlanTA.SharedKernel.CQRS;

namespace PlanTA.Produccion.Application.Features.BOM.DeleteBOM;

public record DeleteBOMCommand(Guid ListaMaterialesId) : ICommand<Guid>;
