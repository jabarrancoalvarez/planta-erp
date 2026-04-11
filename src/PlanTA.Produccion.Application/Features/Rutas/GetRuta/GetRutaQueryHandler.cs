using MediatR;
using Microsoft.EntityFrameworkCore;
using PlanTA.Produccion.Application.DTOs;
using PlanTA.Produccion.Application.Interfaces;
using PlanTA.Produccion.Domain.Entities;
using PlanTA.Produccion.Domain.Errors;
using PlanTA.SharedKernel;

namespace PlanTA.Produccion.Application.Features.Rutas.GetRuta;

public sealed class GetRutaQueryHandler(
    IProduccionDbContext db,
    ICurrentTenant tenant)
    : IRequestHandler<GetRutaQuery, Result<RutaProduccionDetailDto>>
{
    public async Task<Result<RutaProduccionDetailDto>> Handle(GetRutaQuery request, CancellationToken cancellationToken)
    {
        var ruta = await db.RutasProduccion
            .AsNoTracking()
            .Include(r => r.Operaciones)
            .Where(r => r.Id == new RutaProduccionId(request.RutaProduccionId) && r.EmpresaId == tenant.EmpresaId)
            .FirstOrDefaultAsync(cancellationToken);

        if (ruta is null)
            return Result<RutaProduccionDetailDto>.Failure(RutaProduccionErrors.NotFound(request.RutaProduccionId));

        var dto = new RutaProduccionDetailDto(
            ruta.Id.Value,
            ruta.ProductoId,
            ruta.Nombre,
            ruta.Descripcion,
            ruta.Activa,
            ruta.CreatedAt,
            ruta.Operaciones.Select(o => new OperacionRutaDto(
                o.Id.Value,
                o.Numero,
                o.Nombre,
                o.TipoOperacion,
                o.TiempoEstimado.Minutos,
                o.CentroTrabajo,
                o.Instrucciones)).OrderBy(o => o.Numero).ToList());

        return Result<RutaProduccionDetailDto>.Success(dto);
    }
}
