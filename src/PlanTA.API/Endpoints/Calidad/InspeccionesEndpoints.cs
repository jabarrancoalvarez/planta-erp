using MediatR;
using PlanTA.Calidad.Application.Features.Inspecciones.CompletarInspeccion;
using PlanTA.Calidad.Application.Features.Inspecciones.CreateInspeccion;
using PlanTA.Calidad.Application.Features.Inspecciones.GetInspeccion;
using PlanTA.Calidad.Application.Features.Inspecciones.ListInspecciones;
using PlanTA.Calidad.Application.Features.Inspecciones.RegistrarResultado;
using PlanTA.Calidad.Domain.Enums;
using PlanTA.SharedKernel.Extensions;

namespace PlanTA.API.Endpoints.Calidad;

public sealed class InspeccionesEndpoints : IEndpointGroup
{
    public string? RoutePrefix => "/api/v1/calidad/inspecciones";

    public void MapEndpoints(RouteGroupBuilder group)
    {
        group.MapGet("/", async (OrigenInspeccion? origen, ResultadoInspeccion? resultado, int? page, int? pageSize, IMediator m, CancellationToken ct) =>
        {
            var result = await m.Send(new ListInspeccionesQuery(origen, resultado, page ?? 1, pageSize ?? 20), ct);
            return result.ToHttpResult();
        })
        .WithName("ListInspecciones")
        .WithTags("Inspecciones");

        group.MapGet("/{id:guid}", async (Guid id, IMediator m, CancellationToken ct) =>
        {
            var result = await m.Send(new GetInspeccionQuery(id), ct);
            return result.ToHttpResult();
        })
        .WithName("GetInspeccion")
        .WithTags("Inspecciones");

        group.MapPost("/", async (CreateInspeccionCommand command, IMediator m, CancellationToken ct) =>
        {
            var result = await m.Send(command, ct);
            return result.ToHttpResult(201);
        })
        .WithName("CreateInspeccion")
        .WithTags("Inspecciones");

        group.MapPost("/{id:guid}/resultados", async (Guid id, RegistrarResultadoRequest req, IMediator m, CancellationToken ct) =>
        {
            var result = await m.Send(new RegistrarResultadoCommand(
                id, req.CriterioInspeccionId, req.ValorMedido, req.Cumple, req.Observaciones), ct);
            return result.ToHttpResult();
        })
        .WithName("RegistrarResultado")
        .WithTags("Inspecciones");

        group.MapPost("/{id:guid}/completar", async (Guid id, IMediator m, CancellationToken ct) =>
        {
            var result = await m.Send(new CompletarInspeccionCommand(id), ct);
            return result.ToHttpResult();
        })
        .WithName("CompletarInspeccion")
        .WithTags("Inspecciones");
    }
}

public record RegistrarResultadoRequest(
    Guid CriterioInspeccionId,
    string? ValorMedido,
    bool Cumple,
    string? Observaciones = null);
