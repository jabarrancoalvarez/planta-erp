using Microsoft.EntityFrameworkCore;
using PlanTA.Importador.Application.Interfaces;
using PlanTA.Importador.Domain.Entities;

namespace PlanTA.Importador.Infrastructure.Data;

public class ImportadorDbContext : DbContext, IImportadorDbContext
{
    public ImportadorDbContext(DbContextOptions<ImportadorDbContext> options) : base(options) { }

    public DbSet<ImportJob> Jobs => Set<ImportJob>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.HasDefaultSchema("importacion");
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ImportadorDbContext).Assembly);
    }
}
