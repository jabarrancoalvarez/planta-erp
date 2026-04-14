using PlanTA.IA.Domain.Enums;

namespace PlanTA.IA.Application.DTOs;

public record ConversacionListDto(
    Guid Id, string Titulo, ContextoIA Contexto,
    int TotalMensajes, int TotalTokensEntrada, int TotalTokensSalida,
    DateTimeOffset CreatedAt);

public record MensajeIADto(
    Guid Id, RolMensaje Rol, string Contenido,
    int? TokensEntrada, int? TokensSalida, string? Modelo,
    DateTimeOffset CreatedAt);

public record ConversacionDetalleDto(
    Guid Id, string Titulo, ContextoIA Contexto,
    int TotalMensajes, int TotalTokensEntrada, int TotalTokensSalida,
    List<MensajeIADto> Mensajes);

public record PagedResult<T>(List<T> Items, int TotalCount, int Page, int PageSize)
{
    public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);
}
