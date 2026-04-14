using MediatR;
using PlanTA.Calidad.Application.Features.Plantillas.AddCriterio;
using PlanTA.Calidad.Application.Features.Plantillas.CreatePlantilla;
using PlanTA.Calidad.Application.Features.Plantillas.DeletePlantilla;
using PlanTA.Calidad.Application.Features.Plantillas.GetPlantilla;
using PlanTA.Calidad.Application.Features.Plantillas.ListPlantillas;
using PlanTA.Calidad.Application.Features.Plantillas.UpdatePlantilla;
using PlanTA.SharedKernel.Extensions;

namespace PlanTA.API.Endpoints.Calidad;

public sealed class PlantillasEndpoints : IEndpointGroup
{
    public string? RoutePrefix => "/api/v1/calidad/plantillas";

    public void MapEndpoints(RouteGroupBuilder group)
    {
        group.MapGet("/", async (string? search, int? page, int? pageSize, IMediator m, CancellationToken ct) =>
        {
            var result = await m.Send(new ListPlantillasQuery(search, page ?? 1, pageSize ?? 20), ct);
            return result.ToHttpResult();
        })
        .WithName("ListPlantillas")
        .WithTags("Plantillas");

        group.MapGet("/{id:guid}", async (Guid id, IMediator m, CancellationToken ct) =>
        {
            var result = await m.Send(new GetPlantillaQuery(id), ct);
            return result.ToHttpResult();
        })
        .WithName("GetPlantilla")
        .WithTags("Plantillas");

        group.MapPost("/", async (CreatePlantillaCommand command, IMediator m, CancellationToken ct) =>
        {
            var result = await m.Send(command, ct);
            return result.ToHttpResult(201);
        })
        .WithName("CreatePlantilla")
        .WithTags("Plantillas");

        group.MapPost("/{id:guid}/criterios", async (Guid id, AddCriterioRequest req, IMediator m, CancellationToken ct) =>
        {
            var result = await m.Send(new AddCriterioCommand(
                id, req.Nombre, req.TipoMedida, req.EsObligatorio,
                req.Descripcion, req.ValorMinimo, req.ValorMaximo, req.UnidadMedida), ct);
            return result.ToHttpResult(201);
        })
        .WithName("AddCriterio")
        .WithTags("Plantillas");

        group.MapPut("/{id:guid}", async (Guid id, UpdatePlantillaRequest req, IMediator m, CancellationToken ct) =>
        {
            var result = await m.Send(new UpdatePlantillaCommand(id, req.Nombre, req.Descripcion), ct);
            return result.ToHttpResult();
        })
        .WithName("UpdatePlantilla")
        .WithTags("Plantillas");

        group.MapDelete("/{id:guid}", async (Guid id, IMediator m, CancellationToken ct) =>
        {
            var result = await m.Send(new DeletePlantillaCommand(id), ct);
            return result.ToHttpResult();
        })
        .WithName("DeletePlantilla")
        .WithTags("Plantillas");
    }
}

public record UpdatePlantillaRequest(string Nombre, string? Descripcion);

public record AddCriterioRequest(
    string Nombre,
    string TipoMedida,
    bool EsObligatorio,
    string? Descripcion = null,
    decimal? ValorMinimo = null,
    decimal? ValorMaximo = null,
    string? UnidadMedida = null);
