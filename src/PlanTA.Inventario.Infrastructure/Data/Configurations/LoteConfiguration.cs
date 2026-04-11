using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PlanTA.Inventario.Domain.Entities;

namespace PlanTA.Inventario.Infrastructure.Data.Configurations;

public class LoteConfiguration : IEntityTypeConfiguration<Lote>
{
    public void Configure(EntityTypeBuilder<Lote> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasConversion(id => id.Value, v => new LoteId(v));

        builder.Property(x => x.ProductoId).HasConversion(id => id.Value, v => new ProductoId(v));

        builder.OwnsOne(x => x.Codigo, codigo =>
        {
            codigo.Property(c => c.Value).HasColumnName("Codigo").HasMaxLength(50).IsRequired();
            codigo.HasIndex(c => c.Value).IsUnique();
        });

        builder.Property(x => x.CantidadInicial).HasPrecision(18, 4);
        builder.Property(x => x.CantidadActual).HasPrecision(18, 4);
        builder.Property(x => x.Origen).HasMaxLength(200);
        builder.Property(x => x.Notas).HasMaxLength(1000);

        builder.HasIndex(x => x.EmpresaId);
        builder.HasQueryFilter(x => !x.IsDeleted);
    }
}
