namespace PlanTA.SharedKernel;

public interface IHasSoftDelete
{
    bool IsDeleted { get; }
    DateTimeOffset? DeletedAt { get; }
    void SoftDelete();
    void Restore();
}
