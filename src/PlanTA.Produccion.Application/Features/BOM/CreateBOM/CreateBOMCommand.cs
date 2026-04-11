using PlanTA.SharedKernel.CQRS;

namespace PlanTA.Produccion.Application.Features.BOM.CreateBOM;

public record CreateBOMCommand(
    Guid ProductoId,
    string Nombre,
    string? Descripcion = null) : ICommand<Guid>;
