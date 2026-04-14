using MediatR;
using Microsoft.EntityFrameworkCore;
using PlanTA.IA.Application.DTOs;
using PlanTA.IA.Application.Interfaces;
using PlanTA.SharedKernel;

namespace PlanTA.IA.Application.Features.Conversaciones.ListConversaciones;

public sealed class ListConversacionesQueryHandler(
    IIADbContext db,
    ICurrentTenant tenant)
    : IRequestHandler<ListConversacionesQuery, Result<PagedResult<ConversacionListDto>>>
{
    public async Task<Result<PagedResult<ConversacionListDto>>> Handle(
        ListConversacionesQuery request, CancellationToken ct)
    {
        var query = db.Conversaciones.AsNoTracking()
            .Where(c => c.EmpresaId == tenant.EmpresaId);

        if (request.UsuarioId.HasValue)
            query = query.Where(c => c.UsuarioId == request.UsuarioId.Value);

        var total = await query.CountAsync(ct);
        var items = await query
            .OrderByDescending(c => c.CreatedAt)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(c => new ConversacionListDto(
                c.Id.Value, c.Titulo, c.Contexto,
                c.TotalMensajes, c.TotalTokensEntrada, c.TotalTokensSalida,
                c.CreatedAt))
            .ToListAsync(ct);

        return Result<PagedResult<ConversacionListDto>>.Success(
            new PagedResult<ConversacionListDto>(items, total, request.Page, request.PageSize));
    }
}
