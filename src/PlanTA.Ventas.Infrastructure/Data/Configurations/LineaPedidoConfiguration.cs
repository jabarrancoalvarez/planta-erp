using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PlanTA.Ventas.Domain.Entities;

namespace PlanTA.Ventas.Infrastructure.Data.Configurations;

public class LineaPedidoConfiguration : IEntityTypeConfiguration<LineaPedido>
{
    public void Configure(EntityTypeBuilder<LineaPedido> builder)
    {
        builder.ToTable("lineas_pedido");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasConversion(id => id.Value, v => new LineaPedidoId(v));

        builder.Property(x => x.PedidoId)
            .HasConversion(id => id.Value, v => new PedidoId(v));

        builder.Property(x => x.Descripcion).HasMaxLength(500).IsRequired();
        builder.Property(x => x.Cantidad).HasPrecision(18, 4);
        builder.Property(x => x.PrecioUnitario).HasPrecision(18, 4);
        builder.Property(x => x.Descuento).HasPrecision(5, 2);
        builder.Property(x => x.CantidadEnviada).HasPrecision(18, 4);

        builder.HasIndex(x => x.ProductoId);
    }
}
