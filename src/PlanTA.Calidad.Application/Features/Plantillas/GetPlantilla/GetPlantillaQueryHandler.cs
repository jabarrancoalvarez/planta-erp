using MediatR;
using Microsoft.EntityFrameworkCore;
using PlanTA.Calidad.Application.DTOs;
using PlanTA.Calidad.Application.Interfaces;
using PlanTA.Calidad.Domain.Entities;
using PlanTA.Calidad.Domain.Errors;
using PlanTA.SharedKernel;

namespace PlanTA.Calidad.Application.Features.Plantillas.GetPlantilla;

public sealed class GetPlantillaQueryHandler(
    ICalidadDbContext db,
    ICurrentTenant tenant)
    : IRequestHandler<GetPlantillaQuery, Result<PlantillaDetailDto>>
{
    public async Task<Result<PlantillaDetailDto>> Handle(
        GetPlantillaQuery request, CancellationToken cancellationToken)
    {
        var plantilla = await db.PlantillasInspeccion
            .AsNoTracking()
            .Include(p => p.Criterios)
            .Where(p => p.Id == new PlantillaInspeccionId(request.PlantillaId) && p.EmpresaId == tenant.EmpresaId)
            .FirstOrDefaultAsync(cancellationToken);

        if (plantilla is null)
            return Result<PlantillaDetailDto>.Failure(PlantillaErrors.NotFound(request.PlantillaId));

        var dto = new PlantillaDetailDto(
            plantilla.Id.Value,
            plantilla.Nombre,
            plantilla.Descripcion,
            plantilla.OrigenInspeccion,
            plantilla.Version,
            plantilla.Activa,
            plantilla.CreatedAt,
            plantilla.Criterios.OrderBy(c => c.Orden).Select(c => new CriterioDto(
                c.Id.Value, c.Nombre, c.Descripcion, c.TipoMedida,
                c.ValorMinimo, c.ValorMaximo, c.UnidadMedida,
                c.EsObligatorio, c.Orden)).ToList());

        return Result<PlantillaDetailDto>.Success(dto);
    }
}
