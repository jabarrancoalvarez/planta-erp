using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PlanTA.Inventario.Domain.Entities;

namespace PlanTA.Inventario.Infrastructure.Data.Configurations;

public class MovimientoStockConfiguration : IEntityTypeConfiguration<MovimientoStock>
{
    public void Configure(EntityTypeBuilder<MovimientoStock> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasConversion(id => id.Value, v => new MovimientoStockId(v));

        builder.Property(x => x.ProductoId).HasConversion(id => id.Value, v => new ProductoId(v));
        builder.Property(x => x.AlmacenId).HasConversion(id => id.Value, v => new AlmacenId(v));
        builder.Property(x => x.UbicacionId)
            .HasConversion(id => id != null ? id.Value : (Guid?)null, v => v.HasValue ? new UbicacionId(v.Value) : null);
        builder.Property(x => x.LoteId)
            .HasConversion(id => id != null ? id.Value : (Guid?)null, v => v.HasValue ? new LoteId(v.Value) : null);

        builder.Property(x => x.Cantidad).HasPrecision(18, 4);
        builder.Property(x => x.CantidadAnterior).HasPrecision(18, 4);
        builder.Property(x => x.CantidadPosterior).HasPrecision(18, 4);
        builder.Property(x => x.Referencia).HasMaxLength(200);
        builder.Property(x => x.Notas).HasMaxLength(1000);

        builder.HasIndex(x => new { x.ProductoId, x.AlmacenId });
        builder.HasIndex(x => x.EmpresaId);
        builder.HasIndex(x => x.CreatedAt);
    }
}
