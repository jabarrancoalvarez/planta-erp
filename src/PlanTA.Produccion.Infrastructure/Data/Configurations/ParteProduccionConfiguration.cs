using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PlanTA.Produccion.Domain.Entities;

namespace PlanTA.Produccion.Infrastructure.Data.Configurations;

public class ParteProduccionConfiguration : IEntityTypeConfiguration<ParteProduccion>
{
    public void Configure(EntityTypeBuilder<ParteProduccion> builder)
    {
        builder.ToTable("partes_produccion");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasConversion(id => id.Value, v => new ParteProduccionId(v));

        builder.Property(x => x.OrdenFabricacionId)
            .HasConversion(id => id.Value, v => new OrdenFabricacionId(v));

        builder.Property(x => x.UnidadesBuenas).HasPrecision(18, 4);
        builder.Property(x => x.UnidadesDefectuosas).HasPrecision(18, 4);
        builder.Property(x => x.Merma).HasPrecision(18, 4);
        builder.Property(x => x.Observaciones).HasMaxLength(2000);

        builder.HasIndex(x => x.LoteGeneradoId);
    }
}
