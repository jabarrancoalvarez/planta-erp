using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PlanTA.Calidad.Domain.Entities;

namespace PlanTA.Calidad.Infrastructure.Data.Configurations;

public class PlantillaInspeccionConfiguration : IEntityTypeConfiguration<PlantillaInspeccion>
{
    public void Configure(EntityTypeBuilder<PlantillaInspeccion> builder)
    {
        builder.ToTable("plantillas_inspeccion");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasConversion(id => id.Value, v => new PlantillaInspeccionId(v));

        builder.Property(x => x.Nombre).HasMaxLength(300).IsRequired();
        builder.Property(x => x.Descripcion).HasMaxLength(1000);
        builder.Property(x => x.OrigenInspeccion).HasConversion<string>().HasMaxLength(50);

        builder.HasMany(x => x.Criterios)
            .WithOne()
            .HasForeignKey(c => c.PlantillaInspeccionId);

        builder.HasIndex(x => x.EmpresaId);
        builder.HasQueryFilter(x => !x.IsDeleted);
    }
}
