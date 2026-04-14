using PlanTA.SharedKernel.CQRS;

namespace PlanTA.Produccion.Application.Features.Rutas.DeleteRuta;

public record DeleteRutaCommand(Guid RutaProduccionId) : ICommand<Guid>;
