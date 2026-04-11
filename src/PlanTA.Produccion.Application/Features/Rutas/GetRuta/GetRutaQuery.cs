using PlanTA.Produccion.Application.DTOs;
using PlanTA.SharedKernel.CQRS;

namespace PlanTA.Produccion.Application.Features.Rutas.GetRuta;

public record GetRutaQuery(Guid RutaProduccionId) : IQuery<RutaProduccionDetailDto>;
