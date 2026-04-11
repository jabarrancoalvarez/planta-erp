using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PlanTA.Ventas.Domain.Entities;

namespace PlanTA.Ventas.Infrastructure.Data.Configurations;

public class ExpedicionConfiguration : IEntityTypeConfiguration<Expedicion>
{
    public void Configure(EntityTypeBuilder<Expedicion> builder)
    {
        builder.ToTable("expediciones");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasConversion(id => id.Value, v => new ExpedicionId(v));

        builder.Property(x => x.PedidoId)
            .HasConversion(id => id.Value, v => new PedidoId(v));

        builder.Property(x => x.NumeroSeguimiento).HasMaxLength(100);
        builder.Property(x => x.Transportista).HasMaxLength(200);
        builder.Property(x => x.Observaciones).HasMaxLength(2000);

        builder.HasMany(x => x.Lineas)
            .WithOne()
            .HasForeignKey(l => l.ExpedicionId);

        builder.HasIndex(x => x.EmpresaId);
        builder.HasIndex(x => x.PedidoId);
        builder.HasIndex(x => x.EstadoExpedicion);
    }
}
