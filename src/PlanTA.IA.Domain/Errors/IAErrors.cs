using PlanTA.SharedKernel;

namespace PlanTA.IA.Domain.Errors;

public static class ConversacionIAErrors
{
    public static Error NotFound(Guid id) => Error.NotFound("ConversacionIA.NotFound", $"Conversación '{id}' no encontrada");
}

public static class ClaudeErrors
{
    public static Error ApiError(string msg) => Error.Internal("Claude.ApiError", msg);
    public static Error ApiKeyMissing => Error.Internal("Claude.ApiKeyMissing", "Anthropic API key no configurada");
}
