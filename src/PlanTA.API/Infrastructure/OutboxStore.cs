using Microsoft.EntityFrameworkCore;
using PlanTA.SharedKernel.Outbox;

namespace PlanTA.API.Infrastructure;

public sealed class OutboxStore(OutboxDbContext dbContext) : IOutboxStore
{
    public async Task AddAsync(OutboxMessage message, CancellationToken ct = default)
    {
        await dbContext.OutboxMessages.AddAsync(message, ct);
        await dbContext.SaveChangesAsync(ct);
    }

    public async Task<List<OutboxMessage>> GetPendingAsync(int batchSize, CancellationToken ct = default)
    {
        return await dbContext.OutboxMessages
            .Where(m => m.ProcessedAt == null && m.RetryCount < 3)
            .OrderBy(m => m.CreatedAt)
            .Take(batchSize)
            .ToListAsync(ct);
    }

    public async Task MarkAsProcessedAsync(Guid id, CancellationToken ct = default)
    {
        await dbContext.OutboxMessages
            .Where(m => m.Id == id)
            .ExecuteUpdateAsync(s => s
                .SetProperty(m => m.ProcessedAt, DateTimeOffset.UtcNow), ct);
    }

    public async Task MarkAsFailedAsync(Guid id, string error, CancellationToken ct = default)
    {
        await dbContext.OutboxMessages
            .Where(m => m.Id == id)
            .ExecuteUpdateAsync(s => s
                .SetProperty(m => m.Error, error)
                .SetProperty(m => m.RetryCount, m => m.RetryCount + 1), ct);
    }
}
