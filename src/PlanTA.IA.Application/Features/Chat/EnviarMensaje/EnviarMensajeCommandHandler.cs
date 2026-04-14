using MediatR;
using Microsoft.EntityFrameworkCore;
using PlanTA.IA.Application.Interfaces;
using PlanTA.IA.Domain.Entities;
using PlanTA.IA.Domain.Enums;
using PlanTA.IA.Domain.Errors;
using PlanTA.SharedKernel;

namespace PlanTA.IA.Application.Features.Chat.EnviarMensaje;

public sealed class EnviarMensajeCommandHandler(
    IIADbContext db,
    IClaudeService claude,
    ICurrentTenant tenant)
    : IRequestHandler<EnviarMensajeCommand, Result<EnviarMensajeResultDto>>
{
    public async Task<Result<EnviarMensajeResultDto>> Handle(
        EnviarMensajeCommand request, CancellationToken ct)
    {
        ConversacionIA? conv;
        if (request.ConversacionId.HasValue)
        {
            var cid = new ConversacionIAId(request.ConversacionId.Value);
            conv = await db.Conversaciones
                .Include(c => c.Mensajes)
                .FirstOrDefaultAsync(c => c.Id == cid && c.EmpresaId == tenant.EmpresaId, ct);

            if (conv is null)
                return Result<EnviarMensajeResultDto>.Failure(ConversacionIAErrors.NotFound(request.ConversacionId.Value));
        }
        else
        {
            var titulo = request.Titulo ?? TruncarTitulo(request.Mensaje);
            conv = ConversacionIA.Crear(titulo, request.Contexto, request.UsuarioId, tenant.EmpresaId);
            db.Conversaciones.Add(conv);
        }

        conv.AgregarMensaje(RolMensaje.User, request.Mensaje);

        var systemPrompt = BuildSystemPrompt(request.Contexto);
        var historial = conv.Mensajes
            .OrderBy(m => m.CreatedAt)
            .Select(m => new ClaudeMensaje(
                m.Rol == RolMensaje.Assistant ? "assistant" : "user",
                m.Contenido))
            .ToList();

        var res = await claude.EnviarAsync(systemPrompt, historial, ct);
        if (!res.Exito)
            return Result<EnviarMensajeResultDto>.Failure(ClaudeErrors.ApiError(res.Error ?? "Error desconocido"));

        conv.AgregarMensaje(RolMensaje.Assistant, res.Contenido, res.TokensEntrada, res.TokensSalida, res.Modelo);
        await db.SaveChangesAsync(ct);

        return Result<EnviarMensajeResultDto>.Success(new EnviarMensajeResultDto(
            conv.Id.Value, res.Contenido, res.TokensEntrada, res.TokensSalida, res.Modelo));
    }

    private static string TruncarTitulo(string mensaje)
    {
        var t = mensaje.Trim();
        return t.Length > 60 ? t[..60] + "…" : t;
    }

    private static string BuildSystemPrompt(ContextoIA ctx) => ctx switch
    {
        ContextoIA.Inventario => "Eres un asistente experto en gestión de inventario industrial para PYMEs. Responde en español, conciso y práctico.",
        ContextoIA.Produccion => "Eres un asistente experto en producción industrial, BOM y órdenes de fabricación. Responde en español, conciso y práctico.",
        ContextoIA.Compras => "Eres un asistente experto en compras y proveedores industriales. Responde en español, conciso y práctico.",
        ContextoIA.Ventas => "Eres un asistente experto en gestión comercial B2B industrial. Responde en español, conciso y práctico.",
        ContextoIA.Calidad => "Eres un asistente experto en ISO 9001 y calidad industrial. Responde en español, conciso y práctico.",
        ContextoIA.Mantenimiento => "Eres un asistente experto en mantenimiento preventivo y correctivo industrial. Responde en español, conciso y práctico.",
        ContextoIA.RRHH => "Eres un asistente experto en RRHH y legislación laboral española. Responde en español, conciso y práctico.",
        ContextoIA.Facturacion => "Eres un asistente experto en facturación española, IVA y Verifactu. Responde en español, conciso y práctico.",
        _ => "Eres el asistente IA de PlanTA, un ERP/MES para PYMEs industriales. Responde en español, conciso y práctico."
    };
}
