using MediatR;
using PlanTA.Calidad.Application.Features.NoConformidades.AddAccionCorrectiva;
using PlanTA.Calidad.Application.Features.NoConformidades.CambiarEstadoNC;
using PlanTA.Calidad.Application.Features.NoConformidades.CreateNC;
using PlanTA.Calidad.Application.Features.NoConformidades.DeleteNC;
using PlanTA.Calidad.Application.Features.NoConformidades.GetNC;
using PlanTA.Calidad.Application.Features.NoConformidades.ListNCs;
using PlanTA.Calidad.Application.Features.NoConformidades.UpdateNC;
using PlanTA.Calidad.Domain.Enums;
using PlanTA.SharedKernel.Extensions;

namespace PlanTA.API.Endpoints.Calidad;

public sealed class NoConformidadesEndpoints : IEndpointGroup
{
    public string? RoutePrefix => "/api/v1/calidad/no-conformidades";

    public void MapEndpoints(RouteGroupBuilder group)
    {
        group.MapGet("/", async (EstadoNoConformidad? estado, SeveridadNC? severidad, int? page, int? pageSize, IMediator m, CancellationToken ct) =>
        {
            var result = await m.Send(new ListNCsQuery(estado, severidad, page ?? 1, pageSize ?? 20), ct);
            return result.ToHttpResult();
        })
        .WithName("ListNoConformidades")
        .WithTags("NoConformidades");

        group.MapGet("/{id:guid}", async (Guid id, IMediator m, CancellationToken ct) =>
        {
            var result = await m.Send(new GetNCQuery(id), ct);
            return result.ToHttpResult();
        })
        .WithName("GetNoConformidad")
        .WithTags("NoConformidades");

        group.MapPost("/", async (CreateNCCommand command, IMediator m, CancellationToken ct) =>
        {
            var result = await m.Send(command, ct);
            return result.ToHttpResult(201);
        })
        .WithName("CreateNoConformidad")
        .WithTags("NoConformidades");

        group.MapPut("/{id:guid}", async (Guid id, UpdateNCRequest req, IMediator m, CancellationToken ct) =>
        {
            var result = await m.Send(new UpdateNCCommand(id, req.Descripcion, req.Severidad, req.ResponsableUserId), ct);
            return result.ToHttpResult();
        })
        .WithName("UpdateNoConformidad")
        .WithTags("NoConformidades");

        group.MapPut("/{id:guid}/estado", async (Guid id, CambiarEstadoNCRequest req, IMediator m, CancellationToken ct) =>
        {
            var result = await m.Send(new CambiarEstadoNCCommand(id, req.EstadoDestino, req.CausaRaiz), ct);
            return result.ToHttpResult();
        })
        .WithName("CambiarEstadoNC")
        .WithTags("NoConformidades");

        group.MapPost("/{id:guid}/acciones", async (Guid id, AddAccionRequest req, IMediator m, CancellationToken ct) =>
        {
            var result = await m.Send(new AddAccionCorrectivaCommand(
                id, req.Descripcion, req.ResponsableUserId, req.FechaLimite), ct);
            return result.ToHttpResult(201);
        })
        .WithName("AddAccionCorrectiva")
        .WithTags("NoConformidades");

        group.MapDelete("/{id:guid}", async (Guid id, IMediator m, CancellationToken ct) =>
        {
            var result = await m.Send(new DeleteNCCommand(id), ct);
            return result.ToHttpResult();
        })
        .WithName("DeleteNoConformidad")
        .WithTags("NoConformidades");
    }
}

public record UpdateNCRequest(string Descripcion, SeveridadNC Severidad, string? ResponsableUserId);

public record CambiarEstadoNCRequest(EstadoNoConformidad EstadoDestino, string? CausaRaiz = null);

public record AddAccionRequest(
    string Descripcion,
    string? ResponsableUserId = null,
    DateTimeOffset? FechaLimite = null);
