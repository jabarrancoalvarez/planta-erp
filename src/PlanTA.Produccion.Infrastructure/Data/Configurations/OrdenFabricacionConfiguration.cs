using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PlanTA.Produccion.Domain.Entities;

namespace PlanTA.Produccion.Infrastructure.Data.Configurations;

public class OrdenFabricacionConfiguration : IEntityTypeConfiguration<OrdenFabricacion>
{
    public void Configure(EntityTypeBuilder<OrdenFabricacion> builder)
    {
        builder.ToTable("ordenes_fabricacion");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasConversion(id => id.Value, v => new OrdenFabricacionId(v));

        builder.OwnsOne(x => x.CodigoOF, codigo =>
        {
            codigo.Property(c => c.Value).HasColumnName("CodigoOF").HasMaxLength(50).IsRequired();
            codigo.HasIndex(c => c.Value).IsUnique();
        });

        builder.Property(x => x.ListaMaterialesId)
            .HasConversion(id => id.Value, v => new ListaMaterialesId(v));

        builder.Property(x => x.RutaProduccionId)
            .HasConversion(
                id => id != null ? id.Value : (Guid?)null,
                v => v.HasValue ? new RutaProduccionId(v.Value) : null);

        builder.OwnsOne(x => x.CantidadPlanificada, cp =>
        {
            cp.Property(c => c.Cantidad).HasColumnName("CantidadPlanificada").HasPrecision(18, 4);
            cp.Property(c => c.UnidadMedida).HasColumnName("UnidadMedida").HasMaxLength(50).IsRequired();
        });

        builder.Property(x => x.Observaciones).HasMaxLength(2000);

        builder.HasMany(x => x.LineasConsumo)
            .WithOne()
            .HasForeignKey(l => l.OrdenFabricacionId);

        builder.HasMany(x => x.PartesProduccion)
            .WithOne()
            .HasForeignKey(p => p.OrdenFabricacionId);

        builder.HasIndex(x => x.EmpresaId);
        builder.HasIndex(x => x.ProductoId);
        builder.HasIndex(x => x.EstadoOF);
        builder.HasQueryFilter(x => !x.IsDeleted);
    }
}
