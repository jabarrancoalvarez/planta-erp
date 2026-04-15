using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PlanTA.Inventario.Domain.Entities;

namespace PlanTA.Inventario.Infrastructure.Data.Configurations;

public class ProductoConfiguration : IEntityTypeConfiguration<Producto>
{
    public void Configure(EntityTypeBuilder<Producto> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasConversion(id => id.Value, v => new ProductoId(v));

        builder.OwnsOne(x => x.SKU, sku =>
        {
            sku.Property(s => s.Value).HasColumnName("SKU").HasMaxLength(50).IsRequired();
        });

        builder.Property(x => x.Nombre).HasMaxLength(200).IsRequired();
        builder.Property(x => x.Descripcion).HasMaxLength(1000);
        builder.Property(x => x.PrecioCompra).HasPrecision(18, 4);
        builder.Property(x => x.PrecioVenta).HasPrecision(18, 4);
        builder.Property(x => x.PesoKg).HasPrecision(18, 4);
        builder.Property(x => x.CodigoBarras).HasMaxLength(100);
        builder.Property(x => x.ImagenUrl).HasMaxLength(500);

        builder.Property(x => x.CategoriaId)
            .HasConversion(id => id != null ? id.Value : (Guid?)null, v => v.HasValue ? new CategoriaProductoId(v.Value) : null);

        builder.HasIndex(x => x.EmpresaId);
        builder.HasQueryFilter(x => !x.IsDeleted);
    }
}
