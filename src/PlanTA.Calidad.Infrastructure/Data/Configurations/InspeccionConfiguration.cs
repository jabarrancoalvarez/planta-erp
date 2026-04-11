using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PlanTA.Calidad.Domain.Entities;

namespace PlanTA.Calidad.Infrastructure.Data.Configurations;

public class InspeccionConfiguration : IEntityTypeConfiguration<Inspeccion>
{
    public void Configure(EntityTypeBuilder<Inspeccion> builder)
    {
        builder.ToTable("inspecciones");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasConversion(id => id.Value, v => new InspeccionId(v));
        builder.Property(x => x.PlantillaInspeccionId).HasConversion(id => id.Value, v => new PlantillaInspeccionId(v));

        builder.Property(x => x.OrigenInspeccion).HasConversion<string>().HasMaxLength(50);
        builder.Property(x => x.ResultadoInspeccion).HasConversion<string>().HasMaxLength(50);
        builder.Property(x => x.InspectorUserId).HasMaxLength(200);
        builder.Property(x => x.Observaciones).HasMaxLength(2000);

        builder.HasMany(x => x.Resultados)
            .WithOne()
            .HasForeignKey(r => r.InspeccionId);

        builder.HasIndex(x => x.EmpresaId);
        builder.HasIndex(x => x.ReferenciaOrigenId);
    }
}
