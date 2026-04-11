using PlanTA.SharedKernel.CQRS;

namespace PlanTA.Produccion.Application.Features.OrdenesFabricacion.CreateOF;

public record CreateOFCommand(
    string CodigoOF,
    Guid ProductoId,
    Guid ListaMaterialesId,
    decimal CantidadPlanificada,
    string UnidadMedida,
    Guid? RutaProduccionId = null,
    int Prioridad = 0,
    string? Observaciones = null) : ICommand<Guid>;
