using Microsoft.EntityFrameworkCore;
using PlanTA.SharedKernel.Outbox;

namespace PlanTA.API.Infrastructure;

public class OutboxDbContext(DbContextOptions<OutboxDbContext> options) : DbContext(options)
{
    public DbSet<OutboxMessage> OutboxMessages => Set<OutboxMessage>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.HasDefaultSchema("shared");

        modelBuilder.Entity<OutboxMessage>(entity =>
        {
            entity.ToTable("outbox_messages");
            entity.HasKey(e => e.Id);

            entity.Property(e => e.Type)
                .IsRequired()
                .HasMaxLength(500);

            entity.Property(e => e.Content)
                .IsRequired();

            entity.Property(e => e.CreatedAt)
                .IsRequired();

            entity.Property(e => e.Error)
                .HasMaxLength(2000);

            // Indice para el processor: mensajes pendientes ordenados por fecha
            entity.HasIndex(e => new { e.ProcessedAt, e.CreatedAt })
                .HasDatabaseName("ix_outbox_pending");
        });
    }
}
