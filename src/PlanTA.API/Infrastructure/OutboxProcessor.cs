using System.Text.Json;
using MediatR;
using PlanTA.SharedKernel.CQRS;
using PlanTA.SharedKernel.Outbox;

namespace PlanTA.API.Infrastructure;

/// <summary>
/// Background service que procesa mensajes del outbox cada 5 segundos.
/// Deserializa el evento y lo publica via MediatR.
/// Max 3 reintentos por mensaje.
/// </summary>
public sealed class OutboxProcessor(
    IServiceScopeFactory scopeFactory,
    ILogger<OutboxProcessor> logger) : BackgroundService
{
    private const int BatchSize = 20;
    private static readonly TimeSpan PollingInterval = TimeSpan.FromSeconds(5);

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation("OutboxProcessor started");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await ProcessBatchAsync(stoppingToken);
            }
            catch (Exception ex) when (ex is not OperationCanceledException)
            {
                logger.LogError(ex, "Error processing outbox batch");
            }

            await Task.Delay(PollingInterval, stoppingToken);
        }

        logger.LogInformation("OutboxProcessor stopped");
    }

    private async Task ProcessBatchAsync(CancellationToken ct)
    {
        using var scope = scopeFactory.CreateScope();
        var outboxStore = scope.ServiceProvider.GetRequiredService<IOutboxStore>();
        var publisher = scope.ServiceProvider.GetRequiredService<IPublisher>();

        var messages = await outboxStore.GetPendingAsync(BatchSize, ct);
        if (messages.Count == 0) return;

        logger.LogDebug("Processing {Count} outbox messages", messages.Count);

        foreach (var message in messages)
        {
            try
            {
                var eventType = System.Type.GetType(message.Type);
                if (eventType is null)
                {
                    logger.LogWarning(
                        "Could not resolve type {TypeName} for outbox message {MessageId}",
                        message.Type, message.Id);
                    await outboxStore.MarkAsFailedAsync(message.Id, $"Type not found: {message.Type}", ct);
                    continue;
                }

                var domainEvent = JsonSerializer.Deserialize(message.Content, eventType) as IDomainEvent;
                if (domainEvent is null)
                {
                    logger.LogWarning(
                        "Could not deserialize outbox message {MessageId} to {TypeName}",
                        message.Id, message.Type);
                    await outboxStore.MarkAsFailedAsync(message.Id, "Deserialization returned null", ct);
                    continue;
                }

                await publisher.Publish(domainEvent, ct);
                await outboxStore.MarkAsProcessedAsync(message.Id, ct);

                logger.LogDebug("Processed outbox message {MessageId} ({Type})", message.Id, eventType.Name);
            }
            catch (Exception ex) when (ex is not OperationCanceledException)
            {
                logger.LogError(ex,
                    "Error processing outbox message {MessageId}",
                    message.Id);
                await outboxStore.MarkAsFailedAsync(message.Id, ex.Message, ct);
            }
        }
    }
}
