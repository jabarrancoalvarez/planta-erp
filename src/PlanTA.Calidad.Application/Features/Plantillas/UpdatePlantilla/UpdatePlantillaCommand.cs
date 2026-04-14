using MediatR;
using Microsoft.EntityFrameworkCore;
using PlanTA.Calidad.Application.Interfaces;
using PlanTA.Calidad.Domain.Entities;
using PlanTA.Calidad.Domain.Errors;
using PlanTA.SharedKernel;
using PlanTA.SharedKernel.CQRS;

namespace PlanTA.Calidad.Application.Features.Plantillas.UpdatePlantilla;

public record UpdatePlantillaCommand(Guid PlantillaId, string Nombre, string? Descripcion) : ICommand<Guid>;

public sealed class UpdatePlantillaCommandHandler(
    ICalidadDbContext db,
    ICurrentTenant tenant)
    : IRequestHandler<UpdatePlantillaCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(UpdatePlantillaCommand request, CancellationToken cancellationToken)
    {
        var plantilla = await db.PlantillasInspeccion
            .FirstOrDefaultAsync(
                p => p.Id == new PlantillaInspeccionId(request.PlantillaId) && p.EmpresaId == tenant.EmpresaId,
                cancellationToken);

        if (plantilla is null)
            return Result<Guid>.Failure(PlantillaErrors.NotFound(request.PlantillaId));

        plantilla.Actualizar(request.Nombre, request.Descripcion);
        await db.SaveChangesAsync(cancellationToken);
        return Result<Guid>.Success(plantilla.Id.Value);
    }
}
