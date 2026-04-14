using PlanTA.Calidad.Domain.Enums;
using PlanTA.SharedKernel.CQRS;

namespace PlanTA.Calidad.Application.Features.NoConformidades.UpdateNC;

public record UpdateNCCommand(
    Guid NoConformidadId,
    string Descripcion,
    SeveridadNC Severidad,
    string? ResponsableUserId) : ICommand<bool>;
