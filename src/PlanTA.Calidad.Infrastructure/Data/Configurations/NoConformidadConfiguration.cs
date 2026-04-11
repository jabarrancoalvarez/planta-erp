using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PlanTA.Calidad.Domain.Entities;

namespace PlanTA.Calidad.Infrastructure.Data.Configurations;

public class NoConformidadConfiguration : IEntityTypeConfiguration<NoConformidad>
{
    public void Configure(EntityTypeBuilder<NoConformidad> builder)
    {
        builder.ToTable("no_conformidades");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasConversion(id => id.Value, v => new NoConformidadId(v));
        builder.Property(x => x.InspeccionId).HasConversion(
            id => id == null ? (Guid?)null : id.Value,
            v => v.HasValue ? new InspeccionId(v.Value) : null);

        builder.Property(x => x.Codigo).HasMaxLength(50).IsRequired();
        builder.Property(x => x.Descripcion).HasMaxLength(2000).IsRequired();
        builder.Property(x => x.OrigenInspeccion).HasConversion<string>().HasMaxLength(50);
        builder.Property(x => x.SeveridadNC).HasConversion<string>().HasMaxLength(50);
        builder.Property(x => x.EstadoNoConformidad).HasConversion<string>().HasMaxLength(50);
        builder.Property(x => x.CausaRaiz).HasMaxLength(2000);
        builder.Property(x => x.ResponsableUserId).HasMaxLength(200);

        builder.HasMany(x => x.Acciones)
            .WithOne()
            .HasForeignKey(a => a.NoConformidadId);

        builder.HasIndex(x => new { x.Codigo, x.EmpresaId }).IsUnique();
        builder.HasIndex(x => x.EmpresaId);
        builder.HasQueryFilter(x => !x.IsDeleted);
    }
}
