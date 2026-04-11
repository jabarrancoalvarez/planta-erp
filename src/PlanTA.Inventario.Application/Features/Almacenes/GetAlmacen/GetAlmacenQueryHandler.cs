using MediatR;
using Microsoft.EntityFrameworkCore;
using PlanTA.Inventario.Application.DTOs;
using PlanTA.Inventario.Application.Interfaces;
using PlanTA.Inventario.Domain.Entities;
using PlanTA.Inventario.Domain.Errors;
using PlanTA.SharedKernel;

namespace PlanTA.Inventario.Application.Features.Almacenes.GetAlmacen;

public sealed class GetAlmacenQueryHandler(
    IInventarioDbContext db,
    ICurrentTenant tenant)
    : IRequestHandler<GetAlmacenQuery, Result<AlmacenDto>>
{
    public async Task<Result<AlmacenDto>> Handle(GetAlmacenQuery request, CancellationToken cancellationToken)
    {
        var almacen = await db.Almacenes
            .AsNoTracking()
            .Include(a => a.Ubicaciones)
            .Where(a => a.Id == new AlmacenId(request.AlmacenId) && a.EmpresaId == tenant.EmpresaId)
            .FirstOrDefaultAsync(cancellationToken);

        if (almacen is null)
            return Result<AlmacenDto>.Failure(AlmacenErrors.NotFound(request.AlmacenId));

        var dto = new AlmacenDto(
            almacen.Id.Value, almacen.Nombre, almacen.Direccion, almacen.Descripcion,
            almacen.EsPrincipal,
            almacen.Ubicaciones.Select(u => new UbicacionDto(
                u.Id.Value, u.Codigo.ToString(), u.Nombre, u.CapacidadMaxima, u.Activa)).ToList());

        return Result<AlmacenDto>.Success(dto);
    }
}
