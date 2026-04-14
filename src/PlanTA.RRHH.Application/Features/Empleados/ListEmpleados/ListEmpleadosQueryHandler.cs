using MediatR;
using Microsoft.EntityFrameworkCore;
using PlanTA.RRHH.Application.DTOs;
using PlanTA.RRHH.Application.Interfaces;
using PlanTA.SharedKernel;

namespace PlanTA.RRHH.Application.Features.Empleados.ListEmpleados;

public sealed class ListEmpleadosQueryHandler(
    IRRHHDbContext db,
    ICurrentTenant tenant)
    : IRequestHandler<ListEmpleadosQuery, Result<PagedResult<EmpleadoListDto>>>
{
    public async Task<Result<PagedResult<EmpleadoListDto>>> Handle(
        ListEmpleadosQuery request, CancellationToken ct)
    {
        var query = db.Empleados.AsNoTracking()
            .Where(e => e.EmpresaId == tenant.EmpresaId);

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var s = request.Search.ToLowerInvariant();
            query = query.Where(e =>
                e.Nombre.ToLower().Contains(s) ||
                e.Apellidos.ToLower().Contains(s) ||
                e.Codigo.ToLower().Contains(s) ||
                e.DNI.ToLower().Contains(s));
        }

        var total = await query.CountAsync(ct);
        var items = await query
            .OrderBy(e => e.Apellidos).ThenBy(e => e.Nombre)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(e => new EmpleadoListDto(
                e.Id.Value, e.Codigo, e.Nombre, e.Apellidos, e.DNI,
                e.Puesto, e.Departamento, e.FechaBaja == null,
                e.Email, e.Telefono, e.CosteHoraEstandar, e.DiasVacacionesAnuales))
            .ToListAsync(ct);

        return Result<PagedResult<EmpleadoListDto>>.Success(
            new PagedResult<EmpleadoListDto>(items, total, request.Page, request.PageSize));
    }
}
