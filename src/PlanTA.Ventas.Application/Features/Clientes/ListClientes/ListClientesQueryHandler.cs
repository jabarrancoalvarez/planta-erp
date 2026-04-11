using MediatR;
using Microsoft.EntityFrameworkCore;
using PlanTA.Ventas.Application.DTOs;
using PlanTA.Ventas.Application.Interfaces;
using PlanTA.SharedKernel;

namespace PlanTA.Ventas.Application.Features.Clientes.ListClientes;

public sealed class ListClientesQueryHandler(
    IVentasDbContext db,
    ICurrentTenant tenant)
    : IRequestHandler<ListClientesQuery, Result<PagedResult<ClienteListDto>>>
{
    public async Task<Result<PagedResult<ClienteListDto>>> Handle(
        ListClientesQuery request, CancellationToken cancellationToken)
    {
        var query = db.Clientes
            .AsNoTracking()
            .Where(c => c.EmpresaId == tenant.EmpresaId);

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var search = request.Search.ToLowerInvariant();
            query = query.Where(c =>
                c.RazonSocial.ToLower().Contains(search) ||
                c.NIF.ToLower().Contains(search));
        }

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderBy(c => c.RazonSocial)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(c => new ClienteListDto(
                c.Id.Value, c.RazonSocial, c.NIF, c.Ciudad, c.Email, c.Activo))
            .ToListAsync(cancellationToken);

        return Result<PagedResult<ClienteListDto>>.Success(
            new PagedResult<ClienteListDto>(items, totalCount, request.Page, request.PageSize));
    }
}
