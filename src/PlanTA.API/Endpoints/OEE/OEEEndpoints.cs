using MediatR;
using PlanTA.OEE.Application.Features.Registros.DeleteRegistroOEE;
using PlanTA.OEE.Application.Features.Registros.ListRegistros;
using PlanTA.OEE.Application.Features.Registros.RegistrarOEE;
using PlanTA.OEE.Application.Features.Registros.ResumenActivo;
using PlanTA.SharedKernel.Extensions;

namespace PlanTA.API.Endpoints.OEE;

public sealed class OEEEndpoints : IEndpointGroup
{
    public string? RoutePrefix => "/api/v1/oee";

    public void MapEndpoints(RouteGroupBuilder group)
    {
        group.MapGet("/registros", async (Guid? activoId, DateTimeOffset? desde, DateTimeOffset? hasta,
            int? page, int? pageSize, IMediator m, CancellationToken ct) =>
        {
            var r = await m.Send(new ListRegistrosQuery(activoId, desde, hasta, page ?? 1, pageSize ?? 50), ct);
            return r.ToHttpResult();
        }).WithName("ListRegistrosOEE").WithTags("OEE");

        group.MapPost("/registros", async (RegistrarOEECommand cmd, IMediator m, CancellationToken ct) =>
            (await m.Send(cmd, ct)).ToHttpResult(201)).WithName("RegistrarOEE").WithTags("OEE");

        group.MapGet("/activos/{activoId:guid}/resumen", async (Guid activoId, DateTimeOffset? desde,
            DateTimeOffset? hasta, IMediator m, CancellationToken ct) =>
            (await m.Send(new ResumenActivoQuery(activoId, desde, hasta), ct)).ToHttpResult())
            .WithName("ResumenOEEActivo").WithTags("OEE");

        group.MapDelete("/registros/{id:guid}", async (Guid id, IMediator m, CancellationToken ct) =>
            (await m.Send(new DeleteRegistroOEECommand(id), ct)).ToHttpResult())
            .WithName("DeleteRegistroOEE").WithTags("OEE");
    }
}
