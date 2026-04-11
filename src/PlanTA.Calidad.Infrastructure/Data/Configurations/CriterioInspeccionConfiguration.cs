using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PlanTA.Calidad.Domain.Entities;

namespace PlanTA.Calidad.Infrastructure.Data.Configurations;

public class CriterioInspeccionConfiguration : IEntityTypeConfiguration<CriterioInspeccion>
{
    public void Configure(EntityTypeBuilder<CriterioInspeccion> builder)
    {
        builder.ToTable("criterios_inspeccion");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasConversion(id => id.Value, v => new CriterioInspeccionId(v));
        builder.Property(x => x.PlantillaInspeccionId).HasConversion(id => id.Value, v => new PlantillaInspeccionId(v));

        builder.Property(x => x.Nombre).HasMaxLength(300).IsRequired();
        builder.Property(x => x.Descripcion).HasMaxLength(1000);
        builder.Property(x => x.TipoMedida).HasMaxLength(100).IsRequired();
        builder.Property(x => x.ValorMinimo).HasPrecision(18, 4);
        builder.Property(x => x.ValorMaximo).HasPrecision(18, 4);
        builder.Property(x => x.UnidadMedida).HasMaxLength(50);
    }
}
