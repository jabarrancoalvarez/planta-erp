using PlanTA.IA.Application.DTOs;
using PlanTA.SharedKernel.CQRS;

namespace PlanTA.IA.Application.Features.Conversaciones.GetConversacion;

public record GetConversacionQuery(Guid Id) : IQuery<ConversacionDetalleDto>;
