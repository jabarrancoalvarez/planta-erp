using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PlanTA.Facturacion.Domain.Entities;

namespace PlanTA.Facturacion.Infrastructure.Data.Configurations;

public class FacturaConfiguration : IEntityTypeConfiguration<Factura>
{
    public void Configure(EntityTypeBuilder<Factura> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasConversion(id => id.Value, v => new FacturaId(v));

        builder.Property(x => x.NumeroCompleto).HasMaxLength(50).IsRequired();
        builder.Property(x => x.SerieCodigo).HasMaxLength(20).IsRequired();
        builder.Property(x => x.ClienteNombre).HasMaxLength(300).IsRequired();
        builder.Property(x => x.ClienteNIF).HasMaxLength(30);
        builder.Property(x => x.ClienteDireccion).HasMaxLength(500);
        builder.Property(x => x.BaseImponible).HasPrecision(18, 2);
        builder.Property(x => x.TotalIva).HasPrecision(18, 2);
        builder.Property(x => x.Total).HasPrecision(18, 2);
        builder.Property(x => x.Observaciones).HasMaxLength(2000);

        builder.Property(x => x.HashPrevio).HasMaxLength(128);
        builder.Property(x => x.HashActual).HasMaxLength(128);
        builder.Property(x => x.CodigoQrVerifactu).HasMaxLength(500);
        builder.Property(x => x.VerifactuRespuesta).HasMaxLength(4000);
        builder.Property(x => x.VerifactuCsv).HasMaxLength(100);

        builder.HasIndex(x => new { x.EmpresaId, x.NumeroCompleto }).IsUnique();
        builder.HasIndex(x => new { x.EmpresaId, x.Estado });

        builder.HasMany(x => x.Lineas)
            .WithOne()
            .HasForeignKey(l => l.FacturaId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Metadata.FindNavigation(nameof(Factura.Lineas))!
            .SetPropertyAccessMode(PropertyAccessMode.Field);

        builder.HasQueryFilter(x => !x.IsDeleted);
    }
}

public class LineaFacturaConfiguration : IEntityTypeConfiguration<LineaFactura>
{
    public void Configure(EntityTypeBuilder<LineaFactura> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasConversion(id => id.Value, v => new LineaFacturaId(v));
        builder.Property(x => x.FacturaId).HasConversion(id => id.Value, v => new FacturaId(v));

        builder.Property(x => x.Descripcion).HasMaxLength(500).IsRequired();
        builder.Property(x => x.Cantidad).HasPrecision(18, 4);
        builder.Property(x => x.PrecioUnitario).HasPrecision(18, 4);
        builder.Property(x => x.DescuentoPct).HasPrecision(5, 2);
        builder.Property(x => x.IvaPct).HasPrecision(5, 2);

        builder.Ignore(x => x.BaseImponible);
        builder.Ignore(x => x.Iva);
        builder.Ignore(x => x.Total);
    }
}

public class SerieFacturaConfiguration : IEntityTypeConfiguration<SerieFactura>
{
    public void Configure(EntityTypeBuilder<SerieFactura> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasConversion(id => id.Value, v => new SerieFacturaId(v));
        builder.Property(x => x.Codigo).HasMaxLength(20).IsRequired();
        builder.Property(x => x.Descripcion).HasMaxLength(200);
        builder.HasIndex(x => new { x.EmpresaId, x.Codigo, x.Ejercicio }).IsUnique();
        builder.HasQueryFilter(x => !x.IsDeleted);
    }
}
