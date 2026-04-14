using MediatR;
using PlanTA.RRHH.Application.Features.Ausencias.AprobarAusencia;
using PlanTA.RRHH.Application.Features.Ausencias.CreateAusencia;
using PlanTA.RRHH.Application.Features.Ausencias.ListAusencias;
using PlanTA.RRHH.Application.Features.Empleados.CreateEmpleado;
using PlanTA.RRHH.Application.Features.Empleados.ListEmpleados;
using PlanTA.RRHH.Application.Features.Fichajes.ListFichajes;
using PlanTA.RRHH.Application.Features.Fichajes.RegistrarFichaje;
using PlanTA.RRHH.Domain.Enums;
using PlanTA.SharedKernel.Extensions;

namespace PlanTA.API.Endpoints.RRHH;

public sealed class RRHHEndpoints : IEndpointGroup
{
    public string? RoutePrefix => "/api/v1/rrhh";

    public void MapEndpoints(RouteGroupBuilder group)
    {
        // Empleados
        group.MapGet("/empleados", async (string? search, int? page, int? pageSize,
            IMediator m, CancellationToken ct) =>
        {
            var result = await m.Send(new ListEmpleadosQuery(search, page ?? 1, pageSize ?? 20), ct);
            return result.ToHttpResult();
        }).WithName("ListEmpleados").WithTags("RRHH");

        group.MapPost("/empleados", async (CreateEmpleadoCommand cmd, IMediator m, CancellationToken ct) =>
        {
            var result = await m.Send(cmd, ct);
            return result.ToHttpResult(201);
        }).WithName("CreateEmpleado").WithTags("RRHH");

        // Fichajes
        group.MapGet("/fichajes", async (Guid? empleadoId, DateTimeOffset? desde, DateTimeOffset? hasta,
            int? page, int? pageSize, IMediator m, CancellationToken ct) =>
        {
            var result = await m.Send(new ListFichajesQuery(empleadoId, desde, hasta, page ?? 1, pageSize ?? 50), ct);
            return result.ToHttpResult();
        }).WithName("ListFichajes").WithTags("RRHH");

        group.MapPost("/fichajes", async (RegistrarFichajeCommand cmd, IMediator m, CancellationToken ct) =>
        {
            var result = await m.Send(cmd, ct);
            return result.ToHttpResult(201);
        }).WithName("RegistrarFichaje").WithTags("RRHH");

        // Ausencias
        group.MapGet("/ausencias", async (Guid? empleadoId, EstadoAusencia? estado, int? page, int? pageSize,
            IMediator m, CancellationToken ct) =>
        {
            var result = await m.Send(new ListAusenciasQuery(empleadoId, estado, page ?? 1, pageSize ?? 20), ct);
            return result.ToHttpResult();
        }).WithName("ListAusencias").WithTags("RRHH");

        group.MapPost("/ausencias", async (CreateAusenciaCommand cmd, IMediator m, CancellationToken ct) =>
        {
            var result = await m.Send(cmd, ct);
            return result.ToHttpResult(201);
        }).WithName("CreateAusencia").WithTags("RRHH");

        group.MapPost("/ausencias/{id:guid}/aprobar", async (Guid id, AprobarAusenciaRequest req,
            IMediator m, CancellationToken ct) =>
        {
            var result = await m.Send(new AprobarAusenciaCommand(id, req.UserId, true), ct);
            return result.ToHttpResult();
        }).WithName("AprobarAusencia").WithTags("RRHH");

        group.MapPost("/ausencias/{id:guid}/rechazar", async (Guid id, AprobarAusenciaRequest req,
            IMediator m, CancellationToken ct) =>
        {
            var result = await m.Send(new AprobarAusenciaCommand(id, req.UserId, false), ct);
            return result.ToHttpResult();
        }).WithName("RechazarAusencia").WithTags("RRHH");
    }
}

public record AprobarAusenciaRequest(Guid UserId);
