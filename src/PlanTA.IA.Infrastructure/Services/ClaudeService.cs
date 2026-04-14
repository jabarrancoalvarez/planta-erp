using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using PlanTA.IA.Application.Interfaces;

namespace PlanTA.IA.Infrastructure.Services;

public class ClaudeService : IClaudeService
{
    private const string DefaultModel = "claude-sonnet-4-6";
    private readonly HttpClient _http;
    private readonly string? _apiKey;
    private readonly string _model;
    private readonly ILogger<ClaudeService> _logger;

    public ClaudeService(HttpClient http, IConfiguration config, ILogger<ClaudeService> logger)
    {
        _http = http;
        _apiKey = config["Anthropic:ApiKey"];
        _model = config["Anthropic:Model"] ?? DefaultModel;
        _logger = logger;

        _http.BaseAddress = new Uri("https://api.anthropic.com/");
        _http.DefaultRequestHeaders.Add("anthropic-version", "2023-06-01");
        if (!string.IsNullOrWhiteSpace(_apiKey))
            _http.DefaultRequestHeaders.Add("x-api-key", _apiKey);
    }

    public async Task<ClaudeRespuesta> EnviarAsync(
        string systemPrompt,
        IReadOnlyList<ClaudeMensaje> historial,
        CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(_apiKey))
        {
            return new ClaudeRespuesta(
                false, string.Empty, _model, 0, 0,
                "Anthropic API key no configurada. Establecer Anthropic:ApiKey en configuración.");
        }

        var payload = new
        {
            model = _model,
            max_tokens = 1024,
            system = systemPrompt,
            messages = historial.Select(m => new { role = m.Rol, content = m.Contenido }).ToArray()
        };

        try
        {
            var response = await _http.PostAsJsonAsync("v1/messages", payload, ct);
            var json = await response.Content.ReadAsStringAsync(ct);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("Claude API error {Status}: {Body}", response.StatusCode, json);
                return new ClaudeRespuesta(false, string.Empty, _model, 0, 0, $"HTTP {(int)response.StatusCode}: {json}");
            }

            var doc = JsonDocument.Parse(json);
            var root = doc.RootElement;
            var text = root.GetProperty("content")[0].GetProperty("text").GetString() ?? string.Empty;
            var usage = root.GetProperty("usage");
            var inTok = usage.GetProperty("input_tokens").GetInt32();
            var outTok = usage.GetProperty("output_tokens").GetInt32();

            return new ClaudeRespuesta(true, text, _model, inTok, outTok);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error llamando a Claude API");
            return new ClaudeRespuesta(false, string.Empty, _model, 0, 0, ex.Message);
        }
    }
}
