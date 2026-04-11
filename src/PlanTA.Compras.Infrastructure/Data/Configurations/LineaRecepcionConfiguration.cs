using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PlanTA.Compras.Domain.Entities;

namespace PlanTA.Compras.Infrastructure.Data.Configurations;

public class LineaRecepcionConfiguration : IEntityTypeConfiguration<LineaRecepcion>
{
    public void Configure(EntityTypeBuilder<LineaRecepcion> builder)
    {
        builder.ToTable("lineas_recepcion");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasConversion(id => id.Value, v => new LineaRecepcionId(v));

        builder.Property(x => x.RecepcionId)
            .HasConversion(id => id.Value, v => new RecepcionId(v));

        builder.Property(x => x.LineaOrdenCompraId)
            .HasConversion(id => id.Value, v => new LineaOrdenCompraId(v));

        builder.Property(x => x.CantidadRecibida).HasPrecision(18, 4);

        builder.HasIndex(x => x.ProductoId);
        builder.HasIndex(x => x.LineaOrdenCompraId);
    }
}
