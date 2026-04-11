using MediatR;
using PlanTA.Ventas.Application.Features.Expediciones.CambiarEstadoExpedicion;
using PlanTA.Ventas.Application.Features.Expediciones.CreateExpedicion;
using PlanTA.Ventas.Application.Features.Expediciones.GetExpedicion;
using PlanTA.Ventas.Application.Features.Expediciones.ListExpediciones;
using PlanTA.Ventas.Domain.Enums;
using PlanTA.SharedKernel.Extensions;

namespace PlanTA.API.Endpoints.Ventas;

public sealed class ExpedicionesEndpoints : IEndpointGroup
{
    public string? RoutePrefix => "/api/v1/ventas/expediciones";

    public void MapEndpoints(RouteGroupBuilder group)
    {
        group.MapGet("/", async (Guid? pedidoId, EstadoExpedicion? estado, int? page, int? pageSize, IMediator m, CancellationToken ct) =>
        {
            var result = await m.Send(new ListExpedicionesQuery(pedidoId, estado, page ?? 1, pageSize ?? 20), ct);
            return result.ToHttpResult();
        })
        .WithName("ListExpediciones")
        .WithTags("Expediciones");

        group.MapGet("/{id:guid}", async (Guid id, IMediator m, CancellationToken ct) =>
        {
            var result = await m.Send(new GetExpedicionQuery(id), ct);
            return result.ToHttpResult();
        })
        .WithName("GetExpedicion")
        .WithTags("Expediciones");

        group.MapPost("/", async (CreateExpedicionCommand command, IMediator m, CancellationToken ct) =>
        {
            var result = await m.Send(command, ct);
            return result.ToHttpResult(201);
        })
        .WithName("CreateExpedicion")
        .WithTags("Expediciones");

        group.MapPut("/{id:guid}/estado", async (Guid id, CambiarEstadoExpedicionRequest req, IMediator m, CancellationToken ct) =>
        {
            var result = await m.Send(new CambiarEstadoExpedicionCommand(id, req.EstadoDestino), ct);
            return result.ToHttpResult();
        })
        .WithName("CambiarEstadoExpedicion")
        .WithTags("Expediciones");
    }
}

public record CambiarEstadoExpedicionRequest(EstadoExpedicion EstadoDestino);
