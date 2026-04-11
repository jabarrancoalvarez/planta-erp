using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PlanTA.Produccion.Domain.Entities;

namespace PlanTA.Produccion.Infrastructure.Data.Configurations;

public class OperacionRutaConfiguration : IEntityTypeConfiguration<OperacionRuta>
{
    public void Configure(EntityTypeBuilder<OperacionRuta> builder)
    {
        builder.ToTable("operaciones_ruta");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasConversion(id => id.Value, v => new OperacionRutaId(v));

        builder.Property(x => x.RutaProduccionId)
            .HasConversion(id => id.Value, v => new RutaProduccionId(v));

        builder.Property(x => x.Nombre).HasMaxLength(200).IsRequired();
        builder.Property(x => x.CentroTrabajo).HasMaxLength(200).IsRequired();
        builder.Property(x => x.Instrucciones).HasMaxLength(2000);

        builder.OwnsOne(x => x.TiempoEstimado, te =>
        {
            te.Property(t => t.Minutos).HasColumnName("TiempoEstimadoMinutos").HasPrecision(18, 4);
        });
    }
}
