using MediatR;
using Microsoft.EntityFrameworkCore;
using PlanTA.Calidad.Application.Interfaces;
using PlanTA.Calidad.Domain.Entities;
using PlanTA.Calidad.Domain.Errors;
using PlanTA.SharedKernel;

namespace PlanTA.Calidad.Application.Features.Plantillas.DeletePlantilla;

public sealed class DeletePlantillaCommandHandler(
    ICalidadDbContext db,
    ICurrentTenant tenant)
    : IRequestHandler<DeletePlantillaCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(DeletePlantillaCommand request, CancellationToken cancellationToken)
    {
        var plantilla = await db.PlantillasInspeccion
            .FirstOrDefaultAsync(
                p => p.Id == new PlantillaInspeccionId(request.PlantillaId) && p.EmpresaId == tenant.EmpresaId,
                cancellationToken);

        if (plantilla is null)
            return Result<Guid>.Failure(PlantillaErrors.NotFound(request.PlantillaId));

        plantilla.SoftDelete();
        await db.SaveChangesAsync(cancellationToken);

        return Result<Guid>.Success(plantilla.Id.Value);
    }
}
