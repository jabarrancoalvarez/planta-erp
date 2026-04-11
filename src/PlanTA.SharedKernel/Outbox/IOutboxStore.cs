namespace PlanTA.SharedKernel.Outbox;

public interface IOutboxStore
{
    Task AddAsync(OutboxMessage message, CancellationToken ct = default);
    Task<List<OutboxMessage>> GetPendingAsync(int batchSize, CancellationToken ct = default);
    Task MarkAsProcessedAsync(Guid id, CancellationToken ct = default);
    Task MarkAsFailedAsync(Guid id, string error, CancellationToken ct = default);
}
