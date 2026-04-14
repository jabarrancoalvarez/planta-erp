using MediatR;
using Microsoft.EntityFrameworkCore;
using PlanTA.Inventario.Application.DTOs;
using PlanTA.Inventario.Application.Interfaces;
using PlanTA.SharedKernel;

namespace PlanTA.Inventario.Application.Features.Almacenes.ListAlmacenes;

public sealed class ListAlmacenesQueryHandler(
    IInventarioDbContext db,
    ICurrentTenant tenant)
    : IRequestHandler<ListAlmacenesQuery, Result<List<AlmacenListDto>>>
{
    public async Task<Result<List<AlmacenListDto>>> Handle(
        ListAlmacenesQuery request, CancellationToken cancellationToken)
    {
        var almacenes = await db.Almacenes
            .AsNoTracking()
            .Where(a => a.EmpresaId == tenant.EmpresaId)
            .OrderBy(a => a.Nombre)
            .Select(a => new AlmacenListDto(
                a.Id.Value, a.Nombre, a.Direccion, a.EsPrincipal,
                a.Ubicaciones.Count))
            .ToListAsync(cancellationToken);

        return Result<List<AlmacenListDto>>.Success(almacenes);
    }
}
