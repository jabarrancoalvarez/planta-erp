using MediatR;
using Microsoft.EntityFrameworkCore;
using PlanTA.Calidad.Application.Interfaces;
using PlanTA.Calidad.Domain.Entities;
using PlanTA.Calidad.Domain.Errors;
using PlanTA.SharedKernel;

namespace PlanTA.Calidad.Application.Features.Plantillas.AddCriterio;

public sealed class AddCriterioCommandHandler(
    ICalidadDbContext db,
    ICurrentTenant tenant)
    : IRequestHandler<AddCriterioCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(AddCriterioCommand request, CancellationToken cancellationToken)
    {
        var plantilla = await db.PlantillasInspeccion
            .Include(p => p.Criterios)
            .Where(p => p.Id == new PlantillaInspeccionId(request.PlantillaId) && p.EmpresaId == tenant.EmpresaId)
            .FirstOrDefaultAsync(cancellationToken);

        if (plantilla is null)
            return Result<Guid>.Failure(PlantillaErrors.NotFound(request.PlantillaId));

        if (!plantilla.Activa)
            return Result<Guid>.Failure(PlantillaErrors.Inactiva(request.PlantillaId));

        var criterio = plantilla.AgregarCriterio(
            request.Nombre, request.TipoMedida, request.EsObligatorio,
            request.Descripcion, request.ValorMinimo, request.ValorMaximo, request.UnidadMedida);

        await db.SaveChangesAsync(cancellationToken);

        return Result<Guid>.Success(criterio.Id.Value);
    }
}
