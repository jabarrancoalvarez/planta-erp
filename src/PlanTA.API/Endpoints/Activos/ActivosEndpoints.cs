using MediatR;
using PlanTA.Activos.Application.Features.Activos.CambiarEstado;
using PlanTA.Activos.Application.Features.Activos.CreateActivo;
using PlanTA.Activos.Application.Features.Activos.DeleteActivo;
using PlanTA.Activos.Application.Features.Activos.GetActivo;
using PlanTA.Activos.Application.Features.Activos.ListActivos;
using PlanTA.Activos.Application.Features.Activos.UpdateActivo;
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

        group.MapPut("/{id:guid}", async (Guid id, UpdateActivoRequest req, IMediator m, CancellationToken ct) =>
        {
            var result = await m.Send(new UpdateActivoCommand(id, req.Nombre, req.Criticidad,
                req.Descripcion, req.Ubicacion, req.Fabricante, req.Modelo), ct);
            return result.ToHttpResult();
        }).WithName("UpdateActivo").WithTags("Activos");

        group.MapPut("/{id:guid}/estado", async (Guid id, CambiarEstadoRequest req, IMediator m, CancellationToken ct) =>
        {
            var result = await m.Send(new CambiarEstadoActivoCommand(id, req.Estado), ct);
            return result.ToHttpResult();
        }).WithName("CambiarEstadoActivo").WithTags("Activos");

        group.MapDelete("/{id:guid}", async (Guid id, IMediator m, CancellationToken ct) =>
        {
            var result = await m.Send(new DeleteActivoCommand(id), ct);
            return result.ToHttpResult();
        }).WithName("DeleteActivo").WithTags("Activos");
    }
}

public record CambiarEstadoRequest(EstadoActivo Estado);

public record UpdateActivoRequest(
    string Nombre,
    CriticidadActivo Criticidad,
    string? Descripcion = null,
    string? Ubicacion = null,
    string? Fabricante = null,
    string? Modelo = null);
