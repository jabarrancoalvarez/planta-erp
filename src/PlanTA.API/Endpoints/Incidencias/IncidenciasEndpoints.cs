using MediatR;
using PlanTA.Incidencias.Application.Features.Incidencias.CerrarIncidencia;
using PlanTA.Incidencias.Application.Features.Incidencias.CreateIncidencia;
using PlanTA.Incidencias.Application.Features.Incidencias.DeleteIncidencia;
using PlanTA.Incidencias.Application.Features.Incidencias.ListIncidencias;
using PlanTA.Incidencias.Domain.Enums;
using PlanTA.SharedKernel.Extensions;

namespace PlanTA.API.Endpoints.Incidencias;

public sealed class IncidenciasEndpoints : IEndpointGroup
{
    public string? RoutePrefix => "/api/v1/incidencias";

    public void MapEndpoints(RouteGroupBuilder group)
    {
        group.MapGet("/", async (EstadoIncidencia? estado, SeveridadIncidencia? severidad,
            TipoIncidencia? tipo, Guid? activoId, int? page, int? pageSize,
            IMediator m, CancellationToken ct) =>
        {
            var result = await m.Send(new ListIncidenciasQuery(estado, severidad, tipo, activoId, page ?? 1, pageSize ?? 20), ct);
            return result.ToHttpResult();
        }).WithName("ListIncidencias").WithTags("Incidencias");

        group.MapPost("/", async (CreateIncidenciaCommand cmd, IMediator m, CancellationToken ct) =>
        {
            var result = await m.Send(cmd, ct);
            return result.ToHttpResult(201);
        }).WithName("CreateIncidencia").WithTags("Incidencias");

        group.MapPost("/{id:guid}/cerrar", async (Guid id, CerrarIncidenciaRequest req, IMediator m, CancellationToken ct) =>
        {
            var result = await m.Send(new CerrarIncidenciaCommand(id, req.CausaRaiz, req.ResolucionNotas), ct);
            return result.ToHttpResult();
        }).WithName("CerrarIncidencia").WithTags("Incidencias");

        group.MapDelete("/{id:guid}", async (Guid id, IMediator m, CancellationToken ct) =>
        {
            var result = await m.Send(new DeleteIncidenciaCommand(id), ct);
            return result.ToHttpResult();
        }).WithName("DeleteIncidencia").WithTags("Incidencias");
    }
}

public record CerrarIncidenciaRequest(string? CausaRaiz, string? ResolucionNotas);
