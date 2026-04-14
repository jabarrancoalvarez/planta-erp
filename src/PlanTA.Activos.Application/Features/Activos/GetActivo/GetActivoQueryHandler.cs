using MediatR;
using Microsoft.EntityFrameworkCore;
using PlanTA.Activos.Application.DTOs;
using PlanTA.Activos.Application.Interfaces;
using PlanTA.Activos.Domain.Entities;
using PlanTA.Activos.Domain.Errors;
using PlanTA.SharedKernel;

namespace PlanTA.Activos.Application.Features.Activos.GetActivo;

public sealed class GetActivoQueryHandler(
    IActivosDbContext db,
    ICurrentTenant tenant)
    : IRequestHandler<GetActivoQuery, Result<ActivoDto>>
{
    public async Task<Result<ActivoDto>> Handle(GetActivoQuery request, CancellationToken cancellationToken)
    {
        var id = new ActivoId(request.Id);
        var activo = await db.Activos.AsNoTracking()
            .Where(a => a.Id == id && a.EmpresaId == tenant.EmpresaId)
            .Select(a => new ActivoDto(
                a.Id.Value, a.Codigo, a.Nombre, a.Descripcion, a.Tipo, a.Criticidad, a.Estado,
                a.ActivoPadreId != null ? a.ActivoPadreId.Value : (Guid?)null,
                a.Ubicacion, a.Fabricante, a.Modelo, a.NumeroSerie,
                a.FechaAdquisicion, a.CosteAdquisicion, a.HorasUso, a.CreatedAt))
            .FirstOrDefaultAsync(cancellationToken);

        return activo is null
            ? Result<ActivoDto>.Failure(ActivoErrors.NotFound(request.Id))
            : Result<ActivoDto>.Success(activo);
    }
}
