using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PlanTA.Inventario.Domain.Entities;

namespace PlanTA.Inventario.Infrastructure.Data.Configurations;

public class AlertaStockConfiguration : IEntityTypeConfiguration<AlertaStock>
{
    public void Configure(EntityTypeBuilder<AlertaStock> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasConversion(id => id.Value, v => new AlertaStockId(v));

        builder.Property(x => x.ProductoId).HasConversion(id => id.Value, v => new ProductoId(v));
        builder.Property(x => x.AlmacenId)
            .HasConversion(id => id != null ? id.Value : (Guid?)null, v => v.HasValue ? new AlmacenId(v.Value) : null);

        builder.Property(x => x.StockMinimo).HasPrecision(18, 4);
        builder.Property(x => x.StockMaximo).HasPrecision(18, 4);
        builder.Property(x => x.PuntoReorden).HasPrecision(18, 4);
        builder.Property(x => x.CantidadReorden).HasPrecision(18, 4);

        builder.HasIndex(x => new { x.ProductoId, x.AlmacenId });
        builder.HasIndex(x => x.EmpresaId);
    }
}
