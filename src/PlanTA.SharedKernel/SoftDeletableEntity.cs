namespace PlanTA.SharedKernel;

public abstract class SoftDeletableEntity<TId> : AggregateRoot<TId>, IHasSoftDelete
    where TId : EntityId
{
    public bool IsDeleted { get; private set; }
    public DateTimeOffset? DeletedAt { get; private set; }

    public void SoftDelete()
    {
        IsDeleted = true;
        DeletedAt = DateTimeOffset.UtcNow;
    }

    public void Restore()
    {
        IsDeleted = false;
        DeletedAt = null;
    }
}
