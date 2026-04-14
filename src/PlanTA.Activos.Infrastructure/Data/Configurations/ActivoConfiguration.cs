using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PlanTA.Activos.Domain.Entities;

namespace PlanTA.Activos.Infrastructure.Data.Configurations;

public class ActivoConfiguration : IEntityTypeConfiguration<Activo>
{
    public void Configure(EntityTypeBuilder<Activo> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasConversion(id => id.Value, v => new ActivoId(v));

        builder.Property(x => x.Codigo).HasMaxLength(50).IsRequired();
        builder.HasIndex(x => new { x.EmpresaId, x.Codigo }).IsUnique();

        builder.Property(x => x.Nombre).HasMaxLength(200).IsRequired();
        builder.Property(x => x.Descripcion).HasMaxLength(1000);
        builder.Property(x => x.Ubicacion).HasMaxLength(200);
        builder.Property(x => x.Fabricante).HasMaxLength(200);
        builder.Property(x => x.Modelo).HasMaxLength(200);
        builder.Property(x => x.NumeroSerie).HasMaxLength(200);
        builder.Property(x => x.CosteAdquisicion).HasPrecision(18, 4);
        builder.Property(x => x.HorasUso).HasPrecision(18, 4);

        builder.Property(x => x.ActivoPadreId)
            .HasConversion(id => id != null ? id.Value : (Guid?)null,
                v => v.HasValue ? new ActivoId(v.Value) : null);

        builder.HasIndex(x => x.EmpresaId);
        builder.HasQueryFilter(x => !x.IsDeleted);

        builder.HasMany(x => x.Documentos)
            .WithOne()
            .HasForeignKey(d => d.ActivoId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(x => x.Lecturas)
            .WithOne()
            .HasForeignKey(l => l.ActivoId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

public class DocumentoActivoConfiguration : IEntityTypeConfiguration<DocumentoActivo>
{
    public void Configure(EntityTypeBuilder<DocumentoActivo> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasConversion(id => id.Value, v => new DocumentoActivoId(v));
        builder.Property(x => x.ActivoId).HasConversion(id => id.Value, v => new ActivoId(v));
        builder.Property(x => x.Nombre).HasMaxLength(200).IsRequired();
        builder.Property(x => x.Tipo).HasMaxLength(50).IsRequired();
        builder.Property(x => x.Url).HasMaxLength(500).IsRequired();
    }
}

public class LecturaActivoConfiguration : IEntityTypeConfiguration<LecturaActivo>
{
    public void Configure(EntityTypeBuilder<LecturaActivo> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasConversion(id => id.Value, v => new LecturaActivoId(v));
        builder.Property(x => x.ActivoId).HasConversion(id => id.Value, v => new ActivoId(v));
        builder.Property(x => x.Tipo).HasMaxLength(50).IsRequired();
        builder.Property(x => x.Valor).HasPrecision(18, 4);
        builder.Property(x => x.Unidad).HasMaxLength(20);
        builder.Property(x => x.Notas).HasMaxLength(500);
        builder.HasIndex(x => new { x.ActivoId, x.FechaLectura });
    }
}
