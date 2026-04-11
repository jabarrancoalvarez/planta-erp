using Microsoft.EntityFrameworkCore;
using PlanTA.SharedKernel.Audit;

namespace PlanTA.API.Infrastructure;

public class AuditDbContext(DbContextOptions<AuditDbContext> options) : DbContext(options)
{
    public DbSet<AuditEntry> AuditEntries => Set<AuditEntry>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.HasDefaultSchema("shared");

        modelBuilder.Entity<AuditEntry>(entity =>
        {
            entity.ToTable("audit_log");
            entity.HasKey(e => e.Id);

            entity.Property(e => e.UserId)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(e => e.EmpresaId)
                .IsRequired();

            entity.Property(e => e.Action)
                .IsRequired()
                .HasMaxLength(200);

            entity.Property(e => e.EntityType)
                .IsRequired()
                .HasMaxLength(200);

            entity.Property(e => e.EntityId)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(e => e.IpAddress)
                .HasMaxLength(50);

            entity.Property(e => e.Timestamp)
                .IsRequired();

            // Indices para consultas frecuentes
            entity.HasIndex(e => new { e.EntityType, e.EntityId })
                .HasDatabaseName("ix_audit_entity");

            entity.HasIndex(e => e.EmpresaId)
                .HasDatabaseName("ix_audit_empresa");

            entity.HasIndex(e => e.Timestamp)
                .HasDatabaseName("ix_audit_timestamp");
        });
    }
}
