namespace PlanTA.SharedKernel;

public abstract class BaseEntity<TId> where TId : EntityId
{
    public TId Id { get; protected set; } = default!;
    public DateTimeOffset CreatedAt { get; protected set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset? UpdatedAt { get; protected set; }
    public string? CreatedByUserId { get; protected set; }

    public void SetCreatedBy(string userId) => CreatedByUserId = userId;
    public void MarkUpdated() => UpdatedAt = DateTimeOffset.UtcNow;
}
