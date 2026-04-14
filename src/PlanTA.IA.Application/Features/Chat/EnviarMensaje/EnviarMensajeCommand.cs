using PlanTA.IA.Domain.Enums;
using PlanTA.SharedKernel.CQRS;

namespace PlanTA.IA.Application.Features.Chat.EnviarMensaje;

public record EnviarMensajeCommand(
    Guid? ConversacionId,
    string Mensaje,
    Guid UsuarioId,
    ContextoIA Contexto = ContextoIA.General,
    string? Titulo = null) : ICommand<EnviarMensajeResultDto>;

public record EnviarMensajeResultDto(
    Guid ConversacionId,
    string Respuesta,
    int TokensEntrada,
    int TokensSalida,
    string Modelo);
