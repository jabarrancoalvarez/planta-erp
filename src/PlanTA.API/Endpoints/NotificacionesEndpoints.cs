using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using PlanTA.Seguridad.Domain.Entities;
using PlanTA.Seguridad.Infrastructure.Data;
using PlanTA.SharedKernel;

namespace PlanTA.API.Endpoints;

public sealed class NotificacionesEndpoints : IEndpointGroup
{
    public string? RoutePrefix => "/api/v1/notificaciones";

    public void MapEndpoints(RouteGroupBuilder group)
    {
        group.MapGet("/", [Authorize] async (
            SeguridadDbContext db, ICurrentTenant tenant, ClaimsPrincipal user,
            bool soloNoLeidas = false, int take = 50) =>
        {
            var userIdStr = user.FindFirstValue(ClaimTypes.NameIdentifier) ?? user.FindFirstValue("sub");
            Guid.TryParse(userIdStr, out var userId);

            var query = db.Notificaciones
                .Where(n => n.EmpresaId == tenant.EmpresaId)
                .Where(n => n.UsuarioId == null || n.UsuarioId == userId);

            if (soloNoLeidas) query = query.Where(n => !n.Leida);

            var items = await query
                .OrderByDescending(n => n.CreatedAt)
                .Take(Math.Clamp(take, 1, 200))
                .Select(n => new NotificacionDto(
                    n.Id, n.Titulo, n.Mensaje, n.Tipo, n.Url, n.Leida, n.CreatedAt, n.UsuarioId))
                .ToListAsync();

            var noLeidas = await db.Notificaciones
                .Where(n => n.EmpresaId == tenant.EmpresaId)
                .Where(n => n.UsuarioId == null || n.UsuarioId == userId)
                .Where(n => !n.Leida)
                .CountAsync();

            return Results.Ok(new { items, noLeidas });
        })
        .WithName("ListarNotificaciones")
        .WithTags("Notificaciones");

        group.MapPost("/", [Authorize(Roles = "Administrador,GerentePlanta")] async (
            CrearNotificacionRequest req, SeguridadDbContext db, ICurrentTenant tenant) =>
        {
            var notif = Notificacion.Crear(
                tenant.EmpresaId, req.Titulo, req.Mensaje,
                req.Tipo ?? "info", req.UsuarioId, req.Url);
            db.Notificaciones.Add(notif);
            await db.SaveChangesAsync();
            return Results.Ok(new { id = notif.Id });
        })
        .WithName("CrearNotificacion")
        .WithTags("Notificaciones");

        group.MapPost("/{id:guid}/marcar-leida", [Authorize] async (
            Guid id, SeguridadDbContext db, ICurrentTenant tenant) =>
        {
            var n = await db.Notificaciones.FirstOrDefaultAsync(x => x.Id == id && x.EmpresaId == tenant.EmpresaId);
            if (n is null) return Results.NotFound();
            n.MarcarLeida();
            await db.SaveChangesAsync();
            return Results.Ok();
        })
        .WithName("MarcarNotificacionLeida")
        .WithTags("Notificaciones");

        group.MapPost("/marcar-todas-leidas", [Authorize] async (
            SeguridadDbContext db, ICurrentTenant tenant, ClaimsPrincipal user) =>
        {
            var userIdStr = user.FindFirstValue(ClaimTypes.NameIdentifier) ?? user.FindFirstValue("sub");
            Guid.TryParse(userIdStr, out var userId);

            var pendientes = await db.Notificaciones
                .Where(n => n.EmpresaId == tenant.EmpresaId)
                .Where(n => n.UsuarioId == null || n.UsuarioId == userId)
                .Where(n => !n.Leida)
                .ToListAsync();
            foreach (var n in pendientes) n.MarcarLeida();
            await db.SaveChangesAsync();
            return Results.Ok(new { marcadas = pendientes.Count });
        })
        .WithName("MarcarTodasLeidas")
        .WithTags("Notificaciones");
    }
}

public record NotificacionDto(
    Guid Id, string Titulo, string Mensaje, string Tipo, string? Url,
    bool Leida, DateTimeOffset CreatedAt, Guid? UsuarioId);

public record CrearNotificacionRequest(
    string Titulo, string Mensaje, string? Tipo, Guid? UsuarioId, string? Url);
