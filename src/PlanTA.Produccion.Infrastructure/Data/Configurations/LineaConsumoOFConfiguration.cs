using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PlanTA.Produccion.Domain.Entities;

namespace PlanTA.Produccion.Infrastructure.Data.Configurations;

public class LineaConsumoOFConfiguration : IEntityTypeConfiguration<LineaConsumoOF>
{
    public void Configure(EntityTypeBuilder<LineaConsumoOF> builder)
    {
        builder.ToTable("lineas_consumo_of");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasConversion(id => id.Value, v => new LineaConsumoOFId(v));

        builder.Property(x => x.OrdenFabricacionId)
            .HasConversion(id => id.Value, v => new OrdenFabricacionId(v));

        builder.Property(x => x.Cantidad).HasPrecision(18, 4);

        builder.HasIndex(x => x.ProductoId);
        builder.HasIndex(x => x.LoteId);
    }
}
