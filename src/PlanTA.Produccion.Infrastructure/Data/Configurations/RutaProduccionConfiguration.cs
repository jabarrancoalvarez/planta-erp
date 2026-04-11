using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PlanTA.Produccion.Domain.Entities;

namespace PlanTA.Produccion.Infrastructure.Data.Configurations;

public class RutaProduccionConfiguration : IEntityTypeConfiguration<RutaProduccion>
{
    public void Configure(EntityTypeBuilder<RutaProduccion> builder)
    {
        builder.ToTable("rutas_produccion");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasConversion(id => id.Value, v => new RutaProduccionId(v));

        builder.Property(x => x.Nombre).HasMaxLength(200).IsRequired();
        builder.Property(x => x.Descripcion).HasMaxLength(1000);

        builder.HasMany(x => x.Operaciones)
            .WithOne()
            .HasForeignKey(o => o.RutaProduccionId);

        builder.HasIndex(x => x.EmpresaId);
        builder.HasIndex(x => x.ProductoId);
        builder.HasQueryFilter(x => !x.IsDeleted);
    }
}
