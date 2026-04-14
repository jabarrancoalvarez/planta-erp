using PlanTA.IA.Application.DTOs;
using PlanTA.SharedKernel.CQRS;

namespace PlanTA.IA.Application.Features.Conversaciones.ListConversaciones;

public record ListConversacionesQuery(Guid? UsuarioId = null, int Page = 1, int PageSize = 20)
    : IQuery<PagedResult<ConversacionListDto>>;
