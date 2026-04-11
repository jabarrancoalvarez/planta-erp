using Microsoft.EntityFrameworkCore;
using PlanTA.Calidad.Application.Interfaces;
using PlanTA.Calidad.Domain.Entities;

namespace PlanTA.Calidad.Infrastructure.Data;

public class CalidadDbContext : DbContext, ICalidadDbContext
{
    public CalidadDbContext(DbContextOptions<CalidadDbContext> options) : base(options) { }

    public DbSet<PlantillaInspeccion> PlantillasInspeccion => Set<PlantillaInspeccion>();
    public DbSet<CriterioInspeccion> CriteriosInspeccion => Set<CriterioInspeccion>();
    public DbSet<Inspeccion> Inspecciones => Set<Inspeccion>();
    public DbSet<ResultadoCriterio> ResultadosCriterio => Set<ResultadoCriterio>();
    public DbSet<NoConformidad> NoConformidades => Set<NoConformidad>();
    public DbSet<AccionCorrectiva> AccionesCorrectivas => Set<AccionCorrectiva>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.HasDefaultSchema("calidad");
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(CalidadDbContext).Assembly);
    }
}
