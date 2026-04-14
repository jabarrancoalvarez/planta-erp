using MediatR;
using Microsoft.EntityFrameworkCore;
using PlanTA.IA.Application.DTOs;
using PlanTA.IA.Application.Interfaces;
using PlanTA.IA.Domain.Entities;
using PlanTA.IA.Domain.Errors;
using PlanTA.SharedKernel;

namespace PlanTA.IA.Application.Features.Conversaciones.GetConversacion;

public sealed class GetConversacionQueryHandler(
    IIADbContext db,
    ICurrentTenant tenant)
    : IRequestHandler<GetConversacionQuery, Result<ConversacionDetalleDto>>
{
    public async Task<Result<ConversacionDetalleDto>> Handle(GetConversacionQuery request, CancellationToken ct)
    {
        var id = new ConversacionIAId(request.Id);
        var c = await db.Conversaciones.AsNoTracking()
            .Include(x => x.Mensajes)
            .FirstOrDefaultAsync(x => x.Id == id && x.EmpresaId == tenant.EmpresaId, ct);

        if (c is null)
            return Result<ConversacionDetalleDto>.Failure(ConversacionIAErrors.NotFound(request.Id));

        var dto = new ConversacionDetalleDto(
            c.Id.Value, c.Titulo, c.Contexto,
            c.TotalMensajes, c.TotalTokensEntrada, c.TotalTokensSalida,
            c.Mensajes.OrderBy(m => m.CreatedAt).Select(m => new MensajeIADto(
                m.Id.Value, m.Rol, m.Contenido,
                m.TokensEntrada, m.TokensSalida, m.Modelo, m.CreatedAt)).ToList());

        return Result<ConversacionDetalleDto>.Success(dto);
    }
}
