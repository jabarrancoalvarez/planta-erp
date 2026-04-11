using System.Diagnostics;
using MediatR;
using Microsoft.Extensions.Logging;

namespace PlanTA.SharedKernel.Behaviors;

public sealed class PerformanceBehavior<TRequest, TResponse>(
    ILogger<PerformanceBehavior<TRequest, TResponse>> logger)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    private const int WarningThresholdMs = 500;

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        var sw = Stopwatch.StartNew();
        var response = await next();
        sw.Stop();

        if (sw.ElapsedMilliseconds > WarningThresholdMs)
        {
            logger.LogWarning(
                "Long running request: {RequestName} ({ElapsedMs}ms)",
                typeof(TRequest).Name,
                sw.ElapsedMilliseconds);
        }

        return response;
    }
}
