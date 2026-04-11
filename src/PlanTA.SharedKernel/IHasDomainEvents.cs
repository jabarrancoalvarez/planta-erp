using PlanTA.SharedKernel.CQRS;

namespace PlanTA.SharedKernel;

/// <summary>
/// Marker interface (non-generic) para localizar aggregates con domain events
/// desde el DomainEventInterceptor sin conocer el tipo generico TId.
/// </summary>
public interface IHasDomainEvents
{
    IReadOnlyList<IDomainEvent> DomainEvents { get; }
    void ClearDomainEvents();
}
