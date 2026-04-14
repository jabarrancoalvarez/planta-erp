using Microsoft.EntityFrameworkCore;
using PlanTA.Mantenimiento.Application.Interfaces;
using PlanTA.Mantenimiento.Domain.Entities;

namespace PlanTA.Mantenimiento.Infrastructure.Data;

public class MantenimientoDbContext : DbContext, IMantenimientoDbContext
{
    public MantenimientoDbContext(DbContextOptions<MantenimientoDbContext> options) : base(options) { }

    public DbSet<OrdenTrabajo> OrdenesTrabajo => Set<OrdenTrabajo>();
    public DbSet<TareaOT> Tareas => Set<TareaOT>();
    public DbSet<RepuestoOT> Repuestos => Set<RepuestoOT>();
    public DbSet<PlanMantenimiento> Planes => Set<PlanMantenimiento>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.HasDefaultSchema("mantenimiento");
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(MantenimientoDbContext).Assembly);
    }
}
