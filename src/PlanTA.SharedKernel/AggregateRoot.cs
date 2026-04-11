using PlanTA.SharedKernel.CQRS;

namespace PlanTA.SharedKernel;

public abstract class AggregateRoot<TId> : BaseEntity<TId>, IHasDomainEvents where TId : EntityId
{
    private readonly List<IDomainEvent> _domainEvents = [];

    public IReadOnlyList<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    protected void AddDomainEvent(IDomainEvent domainEvent) => _domainEvents.Add(domainEvent);

    public void ClearDomainEvents() => _domainEvents.Clear();

    /// <summary>Concurrencia optimista — EF Core usa esto como concurrency token.</summary>
    public uint Version { get; protected set; }
}
