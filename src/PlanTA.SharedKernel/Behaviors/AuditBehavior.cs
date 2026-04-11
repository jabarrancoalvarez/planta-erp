using System.Text.Json;
using MediatR;
using Microsoft.Extensions.Logging;
using PlanTA.SharedKernel.Audit;
using PlanTA.SharedKernel.CQRS;

namespace PlanTA.SharedKernel.Behaviors;

/// <summary>
/// Pipeline behavior que registra una entrada de auditoria para cada command ejecutado.
/// Solo se activa para ICommand&lt;T&gt;, no para queries.
/// </summary>
public sealed class AuditBehavior<TRequest, TResponse>(
    IAuditStore auditStore,
    ICurrentTenant currentTenant,
    ILogger<AuditBehavior<TRequest, TResponse>> logger)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    private static readonly JsonSerializerOptions SerializerOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = false
    };

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        // Solo auditar commands, no queries
        if (!IsCommand())
            return await next();

        var requestName = typeof(TRequest).Name;
        var response = await next();

        try
        {
            var entry = new AuditEntry
            {
                UserId = currentTenant.UserId.ToString(),
                EmpresaId = currentTenant.EmpresaId,
                Action = requestName,
                EntityType = ExtractEntityType(requestName),
                EntityId = ExtractEntityId(response),
                NewValues = JsonSerializer.Serialize((object)request, SerializerOptions),
                Timestamp = DateTimeOffset.UtcNow
            };

            await auditStore.SaveAsync(entry, cancellationToken);
        }
        catch (Exception ex)
        {
            // La auditoria nunca debe bloquear el flujo principal
            logger.LogError(ex, "Error saving audit entry for {RequestName}", requestName);
        }

        return response;
    }

    private static bool IsCommand()
    {
        return typeof(TRequest).GetInterfaces()
            .Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(ICommand<>));
    }

    /// <summary>
    /// Extrae el nombre de la entidad del nombre del command.
    /// Ejemplo: CreateProductoCommand -> Producto, UpdateOrdenCompraCommand -> OrdenCompra
    /// </summary>
    private static string ExtractEntityType(string commandName)
    {
        var name = commandName;
        // Eliminar sufijos comunes
        foreach (var suffix in new[] { "Command", "Cmd" })
        {
            if (name.EndsWith(suffix, StringComparison.Ordinal))
                name = name[..^suffix.Length];
        }
        // Eliminar prefijos comunes
        foreach (var prefix in new[] { "Create", "Update", "Delete", "Remove", "Add", "Set", "Change" })
        {
            if (name.StartsWith(prefix, StringComparison.Ordinal))
                name = name[prefix.Length..];
        }

        return string.IsNullOrEmpty(name) ? commandName : name;
    }

    private static string ExtractEntityId(TResponse response)
    {
        if (response is null) return string.Empty;

        // Si el response es Result<Guid>, extraer el Value
        var type = response.GetType();
        if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Result<>))
        {
            var isSuccess = (bool)type.GetProperty(nameof(Result<object>.IsSuccess))!.GetValue(response)!;
            if (isSuccess)
            {
                var value = type.GetProperty(nameof(Result<object>.Value))?.GetValue(response);
                return value?.ToString() ?? string.Empty;
            }
        }

        return string.Empty;
    }
}
