namespace PlanTA.IA.Application.Interfaces;

public interface IClaudeService
{
    Task<ClaudeRespuesta> EnviarAsync(
        string systemPrompt,
        IReadOnlyList<ClaudeMensaje> historial,
        CancellationToken ct = default);
}

public record ClaudeMensaje(string Rol, string Contenido);

public record ClaudeRespuesta(
    bool Exito,
    string Contenido,
    string Modelo,
    int TokensEntrada,
    int TokensSalida,
    string? Error = null);
