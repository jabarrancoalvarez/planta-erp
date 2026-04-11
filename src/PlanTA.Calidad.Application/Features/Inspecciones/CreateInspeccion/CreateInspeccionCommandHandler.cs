using MediatR;
using Microsoft.EntityFrameworkCore;
using PlanTA.Calidad.Application.Interfaces;
using PlanTA.Calidad.Domain.Entities;
using PlanTA.Calidad.Domain.Errors;
using PlanTA.SharedKernel;

namespace PlanTA.Calidad.Application.Features.Inspecciones.CreateInspeccion;

public sealed class CreateInspeccionCommandHandler(
    ICalidadDbContext db,
    ICurrentTenant tenant)
    : IRequestHandler<CreateInspeccionCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(CreateInspeccionCommand request, CancellationToken cancellationToken)
    {
        var plantilla = await db.PlantillasInspeccion
            .AsNoTracking()
            .Include(p => p.Criterios)
            .Where(p => p.Id == new PlantillaInspeccionId(request.PlantillaInspeccionId)
                        && p.EmpresaId == tenant.EmpresaId)
            .FirstOrDefaultAsync(cancellationToken);

        if (plantilla is null)
            return Result<Guid>.Failure(PlantillaErrors.NotFound(request.PlantillaInspeccionId));

        if (!plantilla.Activa)
            return Result<Guid>.Failure(PlantillaErrors.Inactiva(request.PlantillaInspeccionId));

        var inspeccion = Inspeccion.Crear(
            plantilla.Id,
            request.OrigenInspeccion,
            request.ReferenciaOrigenId,
            tenant.EmpresaId,
            request.LoteId,
            request.InspectorUserId,
            request.Observaciones);

        // Copy criterios as empty ResultadoCriterio entries
        foreach (var criterio in plantilla.Criterios.OrderBy(c => c.Orden))
        {
            inspeccion.AgregarResultadoVacio(criterio);
        }

        db.Inspecciones.Add(inspeccion);
        await db.SaveChangesAsync(cancellationToken);

        return Result<Guid>.Success(inspeccion.Id.Value);
    }
}
