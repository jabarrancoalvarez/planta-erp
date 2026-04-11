using MediatR;
using PlanTA.Calidad.Application.Interfaces;
using PlanTA.Calidad.Domain.Entities;
using PlanTA.SharedKernel;

namespace PlanTA.Calidad.Application.Features.Plantillas.CreatePlantilla;

public sealed class CreatePlantillaCommandHandler(
    ICalidadDbContext db,
    ICurrentTenant tenant)
    : IRequestHandler<CreatePlantillaCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(CreatePlantillaCommand request, CancellationToken cancellationToken)
    {
        var plantilla = PlantillaInspeccion.Crear(
            request.Nombre, request.OrigenInspeccion, tenant.EmpresaId, request.Descripcion);

        db.PlantillasInspeccion.Add(plantilla);
        await db.SaveChangesAsync(cancellationToken);

        return Result<Guid>.Success(plantilla.Id.Value);
    }
}
