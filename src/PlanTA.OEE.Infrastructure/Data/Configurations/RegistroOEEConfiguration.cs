using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PlanTA.OEE.Domain.Entities;

namespace PlanTA.OEE.Infrastructure.Data.Configurations;

public class RegistroOEEConfiguration : IEntityTypeConfiguration<RegistroOEE>
{
    public void Configure(EntityTypeBuilder<RegistroOEE> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasConversion(id => id.Value, v => new RegistroOEEId(v));

        builder.Property(x => x.TiempoCicloTeoricoSeg).HasPrecision(18, 4);
        builder.Property(x => x.Disponibilidad).HasPrecision(6, 4);
        builder.Property(x => x.Rendimiento).HasPrecision(6, 4);
        builder.Property(x => x.Calidad).HasPrecision(6, 4);
        builder.Property(x => x.OEE).HasPrecision(6, 4);
        builder.Property(x => x.Notas).HasMaxLength(1000);

        builder.HasIndex(x => new { x.EmpresaId, x.ActivoId, x.Fecha });
    }
}
