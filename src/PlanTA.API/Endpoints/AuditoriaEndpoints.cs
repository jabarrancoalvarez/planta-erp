using Microsoft.EntityFrameworkCore;
using PlanTA.API.Infrastructure;
using PlanTA.SharedKernel.Audit;

namespace PlanTA.API.Endpoints;

public class AuditoriaEndpoints : IEndpointGroup
{
    public string? RoutePrefix => "/api/v1/auditoria";

    public void MapEndpoints(RouteGroupBuilder group)
    {
        group.MapGet("/", GetAuditLog)
            .WithName("GetAuditLog")
            .WithSummary("Lista el log de auditoria con filtros opcionales")
            .RequireAuthorization();

        group.MapGet("/entity/{entityType}/{entityId}", GetByEntity)
            .WithName("GetAuditByEntity")
            .WithSummary("Obtiene el historial de auditoria de una entidad especifica")
            .RequireAuthorization();
    }

    private static async Task<IResult> GetAuditLog(
        AuditDbContext dbContext,
        string? entityType = null,
        string? userId = null,
        DateTimeOffset? from = null,
        DateTimeOffset? to = null,
        int page = 1,
        int pageSize = 50)
    {
        var query = dbContext.AuditEntries.AsQueryable();

        if (!string.IsNullOrWhiteSpace(entityType))
            query = query.Where(e => e.EntityType == entityType);

        if (!string.IsNullOrWhiteSpace(userId))
            query = query.Where(e => e.UserId == userId);

        if (from.HasValue)
            query = query.Where(e => e.Timestamp >= from.Value);

        if (to.HasValue)
            query = query.Where(e => e.Timestamp <= to.Value);

        var totalCount = await query.CountAsync();

        var entries = await query
            .OrderByDescending(e => e.Timestamp)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(e => new AuditEntryDto(
                e.Id,
                e.UserId,
                e.EmpresaId,
                e.Action,
                e.EntityType,
                e.EntityId,
                e.OldValues,
                e.NewValues,
                e.Timestamp,
                e.IpAddress))
            .ToListAsync();

        return Results.Ok(new
        {
            Data = entries,
            TotalCount = totalCount,
            Page = page,
            PageSize = pageSize
        });
    }

    private static async Task<IResult> GetByEntity(
        string entityType,
        string entityId,
        IAuditStore auditStore)
    {
        var entries = await auditStore.GetByEntityAsync(entityType, entityId);

        var dtos = entries.Select(e => new AuditEntryDto(
            e.Id,
            e.UserId,
            e.EmpresaId,
            e.Action,
            e.EntityType,
            e.EntityId,
            e.OldValues,
            e.NewValues,
            e.Timestamp,
            e.IpAddress));

        return Results.Ok(dtos);
    }
}

public record AuditEntryDto(
    Guid Id,
    string UserId,
    Guid EmpresaId,
    string Action,
    string EntityType,
    string EntityId,
    string? OldValues,
    string? NewValues,
    DateTimeOffset Timestamp,
    string? IpAddress);
