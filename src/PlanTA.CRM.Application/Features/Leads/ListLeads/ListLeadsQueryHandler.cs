using MediatR;
using Microsoft.EntityFrameworkCore;
using PlanTA.CRM.Application.DTOs;
using PlanTA.CRM.Application.Interfaces;
using PlanTA.SharedKernel;

namespace PlanTA.CRM.Application.Features.Leads.ListLeads;

public sealed class ListLeadsQueryHandler(
    ICrmDbContext db,
    ICurrentTenant tenant)
    : IRequestHandler<ListLeadsQuery, Result<PagedResult<LeadListDto>>>
{
    public async Task<Result<PagedResult<LeadListDto>>> Handle(
        ListLeadsQuery request, CancellationToken ct)
    {
        var query = db.Leads.AsNoTracking()
            .Where(l => l.EmpresaId == tenant.EmpresaId);

        if (request.Estado.HasValue)
            query = query.Where(l => l.Estado == request.Estado.Value);

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var s = request.Search.ToLowerInvariant();
            query = query.Where(l =>
                l.Nombre.ToLower().Contains(s) ||
                (l.Empresa != null && l.Empresa.ToLower().Contains(s)) ||
                (l.Email != null && l.Email.ToLower().Contains(s)));
        }

        var total = await query.CountAsync(ct);
        var items = await query
            .OrderByDescending(l => l.CreatedAt)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(l => new LeadListDto(
                l.Id.Value, l.Nombre, l.Empresa, l.Email, l.Telefono,
                l.Origen, l.Estado, l.AsignadoAUserId, l.Notas))
            .ToListAsync(ct);

        return Result<PagedResult<LeadListDto>>.Success(
            new PagedResult<LeadListDto>(items, total, request.Page, request.PageSize));
    }
}
