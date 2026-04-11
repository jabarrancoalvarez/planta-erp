using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PlanTA.Ventas.Domain.Entities;

namespace PlanTA.Ventas.Infrastructure.Data.Configurations;

public class LineaExpedicionConfiguration : IEntityTypeConfiguration<LineaExpedicion>
{
    public void Configure(EntityTypeBuilder<LineaExpedicion> builder)
    {
        builder.ToTable("lineas_expedicion");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasConversion(id => id.Value, v => new LineaExpedicionId(v));

        builder.Property(x => x.ExpedicionId)
            .HasConversion(id => id.Value, v => new ExpedicionId(v));

        builder.Property(x => x.LineaPedidoId)
            .HasConversion(id => id.Value, v => new LineaPedidoId(v));

        builder.Property(x => x.Cantidad).HasPrecision(18, 4);

        builder.HasIndex(x => x.ProductoId);
        builder.HasIndex(x => x.LineaPedidoId);
    }
}
