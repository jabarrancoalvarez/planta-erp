using Microsoft.EntityFrameworkCore;
using PlanTA.CRM.Application.Interfaces;
using PlanTA.CRM.Domain.Entities;

namespace PlanTA.CRM.Infrastructure.Data;

public class CrmDbContext : DbContext, ICrmDbContext
{
    public CrmDbContext(DbContextOptions<CrmDbContext> options) : base(options) { }

    public DbSet<Lead> Leads => Set<Lead>();
    public DbSet<Oportunidad> Oportunidades => Set<Oportunidad>();
    public DbSet<ActividadCrm> Actividades => Set<ActividadCrm>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.HasDefaultSchema("crm");
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(CrmDbContext).Assembly);
    }
}
