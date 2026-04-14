using Microsoft.EntityFrameworkCore;
using PlanTA.RRHH.Application.Interfaces;
using PlanTA.RRHH.Domain.Entities;

namespace PlanTA.RRHH.Infrastructure.Data;

public class RRHHDbContext : DbContext, IRRHHDbContext
{
    public RRHHDbContext(DbContextOptions<RRHHDbContext> options) : base(options) { }

    public DbSet<Empleado> Empleados => Set<Empleado>();
    public DbSet<Turno> Turnos => Set<Turno>();
    public DbSet<Fichaje> Fichajes => Set<Fichaje>();
    public DbSet<Ausencia> Ausencias => Set<Ausencia>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.HasDefaultSchema("rrhh");
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(RRHHDbContext).Assembly);
    }
}
