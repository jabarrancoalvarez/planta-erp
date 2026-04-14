using Microsoft.EntityFrameworkCore;
using PlanTA.Costes.Application.Interfaces;
using PlanTA.Costes.Domain.Entities;

namespace PlanTA.Costes.Infrastructure.Data;

public class CostesDbContext : DbContext, ICostesDbContext
{
    public CostesDbContext(DbContextOptions<CostesDbContext> options) : base(options) { }

    public DbSet<ImputacionCoste> Imputaciones => Set<ImputacionCoste>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.HasDefaultSchema("costes");
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(CostesDbContext).Assembly);
    }
}
