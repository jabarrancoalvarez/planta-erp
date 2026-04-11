using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PlanTA.Compras.Domain.Entities;

namespace PlanTA.Compras.Infrastructure.Data.Configurations;

public class LineaOrdenCompraConfiguration : IEntityTypeConfiguration<LineaOrdenCompra>
{
    public void Configure(EntityTypeBuilder<LineaOrdenCompra> builder)
    {
        builder.ToTable("lineas_orden_compra");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasConversion(id => id.Value, v => new LineaOrdenCompraId(v));

        builder.Property(x => x.OrdenCompraId)
            .HasConversion(id => id.Value, v => new OrdenCompraId(v));

        builder.Property(x => x.Descripcion).HasMaxLength(500).IsRequired();
        builder.Property(x => x.Cantidad).HasPrecision(18, 4);
        builder.Property(x => x.PrecioUnitario).HasPrecision(18, 4);
        builder.Property(x => x.CantidadRecibida).HasPrecision(18, 4);

        builder.HasIndex(x => x.ProductoId);
    }
}
