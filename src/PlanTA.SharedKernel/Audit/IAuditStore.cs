namespace PlanTA.SharedKernel.Audit;

public interface IAuditStore
{
    Task SaveAsync(AuditEntry entry, CancellationToken ct = default);
    Task<List<AuditEntry>> GetByEntityAsync(string entityType, string entityId, CancellationToken ct = default);
}
