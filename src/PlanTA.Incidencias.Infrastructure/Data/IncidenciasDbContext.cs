using Microsoft.EntityFrameworkCore;
using PlanTA.Incidencias.Application.Interfaces;
using PlanTA.Incidencias.Domain.Entities;

namespace PlanTA.Incidencias.Infrastructure.Data;

public class IncidenciasDbContext : DbContext, IIncidenciasDbContext
{
    public IncidenciasDbContext(DbContextOptions<IncidenciasDbContext> options) : base(options) { }

    public DbSet<Incidencia> Incidencias => Set<Incidencia>();
    public DbSet<FotoIncidencia> Fotos => Set<FotoIncidencia>();
    public DbSet<ComentarioIncidencia> Comentarios => Set<ComentarioIncidencia>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.HasDefaultSchema("incidencias");
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(IncidenciasDbContext).Assembly);
    }
}
