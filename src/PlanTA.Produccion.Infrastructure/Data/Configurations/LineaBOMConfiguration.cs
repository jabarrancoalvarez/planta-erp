using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PlanTA.Produccion.Domain.Entities;

namespace PlanTA.Produccion.Infrastructure.Data.Configurations;

public class LineaBOMConfiguration : IEntityTypeConfiguration<LineaBOM>
{
    public void Configure(EntityTypeBuilder<LineaBOM> builder)
    {
        builder.ToTable("lineas_bom");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasConversion(id => id.Value, v => new LineaBOMId(v));

        builder.Property(x => x.ListaMaterialesId)
            .HasConversion(id => id.Value, v => new ListaMaterialesId(v));

        builder.Property(x => x.Cantidad).HasPrecision(18, 4);
        builder.Property(x => x.UnidadMedida).HasMaxLength(50).IsRequired();
        builder.Property(x => x.Merma).HasPrecision(18, 4);

        builder.HasIndex(x => x.ComponenteProductoId);
    }
}
