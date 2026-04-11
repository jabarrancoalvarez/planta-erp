using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PlanTA.Inventario.Domain.Entities;

namespace PlanTA.Inventario.Infrastructure.Data.Configurations;

public class UbicacionConfiguration : IEntityTypeConfiguration<Ubicacion>
{
    public void Configure(EntityTypeBuilder<Ubicacion> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasConversion(id => id.Value, v => new UbicacionId(v));

        builder.Property(x => x.AlmacenId).HasConversion(id => id.Value, v => new AlmacenId(v));

        builder.OwnsOne(x => x.Codigo, codigo =>
        {
            codigo.Property(c => c.Pasillo).HasColumnName("Pasillo").HasMaxLength(10).IsRequired();
            codigo.Property(c => c.Estante).HasColumnName("Estante").HasMaxLength(10).IsRequired();
            codigo.Property(c => c.Nivel).HasColumnName("Nivel").HasMaxLength(10).IsRequired();
        });

        builder.Property(x => x.Nombre).HasMaxLength(200);
        builder.HasIndex(x => x.EmpresaId);
    }
}
