using MediatR;
using Microsoft.EntityFrameworkCore;
using PlanTA.RRHH.Application.Interfaces;
using PlanTA.SharedKernel;
using PlanTA.RRHH.Application.Features.Ausencias.AprobarAusencia;
using PlanTA.RRHH.Application.Features.Ausencias.CreateAusencia;
using PlanTA.RRHH.Application.Features.Ausencias.DeleteAusencia;
using PlanTA.RRHH.Application.Features.Ausencias.ListAusencias;
using PlanTA.RRHH.Application.Features.Ausencias.UpdateAusencia;
using PlanTA.RRHH.Application.Features.Empleados.CreateEmpleado;
using PlanTA.RRHH.Application.Features.Empleados.DeleteEmpleado;
using PlanTA.RRHH.Application.Features.Empleados.ListEmpleados;
using PlanTA.RRHH.Application.Features.Empleados.UpdateEmpleado;
using PlanTA.RRHH.Application.Features.Fichajes.DeleteFichaje;
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

        group.MapPut("/empleados/{id:guid}", async (Guid id, UpdateEmpleadoRequest req, IMediator m, CancellationToken ct) =>
            (await m.Send(new UpdateEmpleadoCommand(id, req.Nombre, req.Apellidos, req.Puesto,
                req.Email, req.Telefono, req.Departamento, req.CosteHoraEstandar, req.DiasVacacionesAnuales), ct)).ToHttpResult())
            .WithName("UpdateEmpleado").WithTags("RRHH");

        group.MapDelete("/empleados/{id:guid}", async (Guid id, IMediator m, CancellationToken ct) =>
            (await m.Send(new DeleteEmpleadoCommand(id), ct)).ToHttpResult())
            .WithName("DeleteEmpleado").WithTags("RRHH");

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

        group.MapGet("/empleados/me", async (IRRHHDbContext db, ICurrentTenant tenant, CancellationToken ct) =>
        {
            var empleado = await db.Empleados.AsNoTracking()
                .Where(e => e.UserId == tenant.UserId)
                .Select(e => new { id = e.Id.Value, nombre = e.Nombre, apellidos = e.Apellidos, email = e.Email, puesto = e.Puesto })
                .FirstOrDefaultAsync(ct);
            return empleado is null ? Results.NotFound() : Results.Ok(empleado);
        }).WithName("GetEmpleadoMe").WithTags("RRHH");

        group.MapPost("/fichajes/me", async (FichajeMeRequest req, IRRHHDbContext db, IMediator m, ICurrentTenant tenant, CancellationToken ct) =>
        {
            var empleadoId = await db.Empleados.AsNoTracking()
                .Where(e => e.UserId == tenant.UserId)
                .Select(e => e.Id.Value)
                .FirstOrDefaultAsync(ct);
            if (empleadoId == Guid.Empty)
                return Results.NotFound(new { message = "No hay empleado vinculado al usuario actual" });

            var result = await m.Send(new RegistrarFichajeCommand(empleadoId, req.Tipo, Notas: req.Notas), ct);
            return result.ToHttpResult(201);
        }).WithName("RegistrarFichajeMe").WithTags("RRHH");

        group.MapDelete("/fichajes/{id:guid}", async (Guid id, IMediator m, CancellationToken ct) =>
            (await m.Send(new DeleteFichajeCommand(id), ct)).ToHttpResult())
            .WithName("DeleteFichaje").WithTags("RRHH");

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

        group.MapPut("/ausencias/{id:guid}", async (Guid id, UpdateAusenciaRequest req, IMediator m, CancellationToken ct) =>
            (await m.Send(new UpdateAusenciaCommand(id, req.Tipo, req.FechaInicio, req.FechaFin, req.Motivo), ct)).ToHttpResult())
            .WithName("UpdateAusencia").WithTags("RRHH");

        group.MapDelete("/ausencias/{id:guid}", async (Guid id, IMediator m, CancellationToken ct) =>
            (await m.Send(new DeleteAusenciaCommand(id), ct)).ToHttpResult())
            .WithName("DeleteAusencia").WithTags("RRHH");

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
public record FichajeMeRequest(PlanTA.RRHH.Domain.Enums.TipoFichaje Tipo, string? Notas = null);
public record UpdateEmpleadoRequest(
    string Nombre, string Apellidos, string Puesto,
    string? Email, string? Telefono, string? Departamento,
    decimal CosteHoraEstandar, int DiasVacacionesAnuales);
public record UpdateAusenciaRequest(
    TipoAusencia Tipo, DateTimeOffset FechaInicio, DateTimeOffset FechaFin, string? Motivo);
