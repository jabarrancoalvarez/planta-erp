using MediatR;
using PlanTA.Activos.Application.Features.Activos.CambiarEstado;
using PlanTA.Activos.Application.Features.Activos.CreateActivo;
using PlanTA.Activos.Application.Features.Activos.GetActivo;
using PlanTA.Activos.Application.Features.Activos.ListActivos;
using PlanTA.Activos.Domain.Enums;
using PlanTA.SharedKernel.Extensions;

namespace PlanTA.API.Endpoints.Activos;

public sealed class ActivosEndpoints : IEndpointGroup
{
    public string? RoutePrefix => "/api/v1/activos";

    public void MapEndpoints(RouteGroupBuilder group)
    {
        group.MapGet("/", async (string? search, TipoActivo? tipo, EstadoActivo? estado,
            CriticidadActivo? criticidad, int? page, int? pageSize, IMediator m, CancellationToken ct) =>
        {
            var result = await m.Send(new ListActivosQuery(search, tipo, estado, criticidad, page ?? 1, pageSize ?? 20), ct);
            return result.ToHttpResult();
        }).WithName("ListActivos").WithTags("Activos");

        group.MapGet("/{id:guid}", async (Guid id, IMediator m, CancellationToken ct) =>
        {
            var result = await m.Send(new GetActivoQuery(id), ct);
            return result.ToHttpResult();
        }).WithName("GetActivo").WithTags("Activos");

        group.MapPost("/", async (CreateActivoCommand cmd, IMediator m, CancellationToken ct) =>
        {
            var result = await m.Send(cmd, ct);
            return result.ToHttpResult(201);
        }).WithName("CreateActivo").WithTags("Activos");

        group.MapPut("/{id:guid}/estado", async (Guid id, CambiarEstadoRequest req, IMediator m, CancellationToken ct) =>
        {
            var result = await m.Send(new CambiarEstadoActivoCommand(id, req.Estado), ct);
            return result.ToHttpResult();
        }).WithName("CambiarEstadoActivo").WithTags("Activos");
    }
}

public record CambiarEstadoRequest(EstadoActivo Estado);
