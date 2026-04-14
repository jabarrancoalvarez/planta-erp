using Microsoft.EntityFrameworkCore;
using PlanTA.OEE.Application.Interfaces;
using PlanTA.OEE.Domain.Entities;

namespace PlanTA.OEE.Infrastructure.Data;

public class OEEDbContext : DbContext, IOEEDbContext
{
    public OEEDbContext(DbContextOptions<OEEDbContext> options) : base(options) { }

    public DbSet<RegistroOEE> Registros => Set<RegistroOEE>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.HasDefaultSchema("oee");
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(OEEDbContext).Assembly);
    }
}
