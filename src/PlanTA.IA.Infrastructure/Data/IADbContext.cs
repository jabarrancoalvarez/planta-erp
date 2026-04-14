using Microsoft.EntityFrameworkCore;
using PlanTA.IA.Application.Interfaces;
using PlanTA.IA.Domain.Entities;

namespace PlanTA.IA.Infrastructure.Data;

public class IADbContext : DbContext, IIADbContext
{
    public IADbContext(DbContextOptions<IADbContext> options) : base(options) { }

    public DbSet<ConversacionIA> Conversaciones => Set<ConversacionIA>();
    public DbSet<MensajeIA> Mensajes => Set<MensajeIA>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.HasDefaultSchema("ia");
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(IADbContext).Assembly);
    }
}
