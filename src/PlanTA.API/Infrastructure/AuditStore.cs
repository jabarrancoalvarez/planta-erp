using Microsoft.EntityFrameworkCore;
using PlanTA.SharedKernel.Audit;

namespace PlanTA.API.Infrastructure;

public sealed class AuditStore(AuditDbContext dbContext) : IAuditStore
{
    public async Task SaveAsync(AuditEntry entry, CancellationToken ct = default)
    {
        await dbContext.AuditEntries.AddAsync(entry, ct);
        await dbContext.SaveChangesAsync(ct);
    }

    public async Task<List<AuditEntry>> GetByEntityAsync(
        string entityType,
        string entityId,
        CancellationToken ct = default)
    {
        return await dbContext.AuditEntries
            .Where(e => e.EntityType == entityType && e.EntityId == entityId)
            .OrderByDescending(e => e.Timestamp)
            .ToListAsync(ct);
    }
}
