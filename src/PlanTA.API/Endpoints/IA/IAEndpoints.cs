using MediatR;
using PlanTA.IA.Application.Features.Chat.EnviarMensaje;
using PlanTA.IA.Application.Features.Conversaciones.GetConversacion;
using PlanTA.IA.Application.Features.Conversaciones.ListConversaciones;
using PlanTA.SharedKernel.Extensions;

namespace PlanTA.API.Endpoints.IA;

public sealed class IAEndpoints : IEndpointGroup
{
    public string? RoutePrefix => "/api/v1/ia";

    public void MapEndpoints(RouteGroupBuilder group)
    {
        group.MapPost("/chat", async (EnviarMensajeCommand cmd, IMediator m, CancellationToken ct) =>
            (await m.Send(cmd, ct)).ToHttpResult()).WithName("EnviarMensajeIA").WithTags("IA");

        group.MapGet("/conversaciones", async (Guid? usuarioId, int? page, int? pageSize,
            IMediator m, CancellationToken ct) =>
        {
            var r = await m.Send(new ListConversacionesQuery(usuarioId, page ?? 1, pageSize ?? 20), ct);
            return r.ToHttpResult();
        }).WithName("ListConversacionesIA").WithTags("IA");

        group.MapGet("/conversaciones/{id:guid}", async (Guid id, IMediator m, CancellationToken ct) =>
            (await m.Send(new GetConversacionQuery(id), ct)).ToHttpResult())
            .WithName("GetConversacionIA").WithTags("IA");
    }
}
