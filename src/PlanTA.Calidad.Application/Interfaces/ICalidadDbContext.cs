using Microsoft.EntityFrameworkCore;
using PlanTA.Calidad.Domain.Entities;

namespace PlanTA.Calidad.Application.Interfaces;

public interface ICalidadDbContext
{
    DbSet<PlantillaInspeccion> PlantillasInspeccion { get; }
    DbSet<CriterioInspeccion> CriteriosInspeccion { get; }
    DbSet<Inspeccion> Inspecciones { get; }
    DbSet<ResultadoCriterio> ResultadosCriterio { get; }
    DbSet<NoConformidad> NoConformidades { get; }
    DbSet<AccionCorrectiva> AccionesCorrectivas { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
