using PlanTA.Calidad.Domain.Enums;
using PlanTA.SharedKernel.CQRS;

namespace PlanTA.Calidad.Application.Features.NoConformidades.CreateNC;

public record CreateNCCommand(
    string Codigo,
    OrigenInspeccion OrigenInspeccion,
    Guid ReferenciaOrigenId,
    string Descripcion,
    SeveridadNC SeveridadNC,
    Guid? InspeccionId = null,
    string? ResponsableUserId = null) : ICommand<Guid>;
