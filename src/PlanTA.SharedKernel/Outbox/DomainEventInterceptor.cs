using System.Text.Json;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using PlanTA.SharedKernel.CQRS;

namespace PlanTA.SharedKernel.Outbox;

/// <summary>
/// EF Core SaveChanges interceptor que captura domain events de los aggregates
/// y los persiste en la outbox table dentro de la misma transaccion.
/// Registrado como singleton y resuelve IOutboxStore via IServiceScopeFactory
/// para evitar problemas de scope lifetime.
/// </summary>
public sealed class DomainEventInterceptor(IServiceScopeFactory scopeFactory) : SaveChangesInterceptor
{
    private static readonly JsonSerializerOptions SerializerOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = false
    };

    public override async ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        var context = eventData.Context;
        if (context is null)
            return await base.SavingChangesAsync(eventData, result, cancellationToken);

        var aggregates = context.ChangeTracker
            .Entries<IHasDomainEvents>()
            .Where(e => e.Entity.DomainEvents.Count > 0)
            .Select(e => e.Entity)
            .ToList();

        if (aggregates.Count == 0)
            return await base.SavingChangesAsync(eventData, result, cancellationToken);

        var events = aggregates
            .SelectMany(a => a.DomainEvents)
            .ToList();

        using var scope = scopeFactory.CreateScope();
        var outboxStore = scope.ServiceProvider.GetRequiredService<IOutboxStore>();

        foreach (var domainEvent in events)
        {
            var outboxMessage = new OutboxMessage
            {
                Type = domainEvent.GetType().AssemblyQualifiedName!,
                Content = JsonSerializer.Serialize((object)domainEvent, SerializerOptions),
                CreatedAt = DateTimeOffset.UtcNow
            };

            await outboxStore.AddAsync(outboxMessage, cancellationToken);
        }

        foreach (var aggregate in aggregates)
        {
            aggregate.ClearDomainEvents();
        }

        return await base.SavingChangesAsync(eventData, result, cancellationToken);
    }
}
