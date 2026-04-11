using PlanTA.Compras.Application.DTOs;
using PlanTA.SharedKernel.CQRS;

namespace PlanTA.Compras.Application.Features.Recepciones.GetRecepcion;

public record GetRecepcionQuery(Guid RecepcionId) : IQuery<RecepcionDetailDto>;
