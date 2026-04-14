using Microsoft.EntityFrameworkCore;
using PlanTA.Activos.Application.Interfaces;
using PlanTA.Activos.Domain.Entities;

namespace PlanTA.Activos.Infrastructure.Data;

public class ActivosDbContext : DbContext, IActivosDbContext
{
    public ActivosDbContext(DbContextOptions<ActivosDbContext> options) : base(options) { }

    public DbSet<Activo> Activos => Set<Activo>();
    public DbSet<DocumentoActivo> Documentos => Set<DocumentoActivo>();
    public DbSet<LecturaActivo> Lecturas => Set<LecturaActivo>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.HasDefaultSchema("activos");
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ActivosDbContext).Assembly);
    }
}
