using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PlanTA.Incidencias.Domain.Entities;

namespace PlanTA.Incidencias.Infrastructure.Data.Configurations;

public class IncidenciaConfiguration : IEntityTypeConfiguration<Incidencia>
{
    public void Configure(EntityTypeBuilder<Incidencia> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasConversion(id => id.Value, v => new IncidenciaId(v));

        builder.Property(x => x.Codigo).HasMaxLength(50).IsRequired();
        builder.HasIndex(x => new { x.EmpresaId, x.Codigo }).IsUnique();

        builder.Property(x => x.Titulo).HasMaxLength(200).IsRequired();
        builder.Property(x => x.Descripcion).HasMaxLength(4000).IsRequired();
        builder.Property(x => x.UbicacionTexto).HasMaxLength(200);
        builder.Property(x => x.CausaRaiz).HasMaxLength(2000);
        builder.Property(x => x.ResolucionNotas).HasMaxLength(2000);

        builder.HasIndex(x => x.EmpresaId);
        builder.HasIndex(x => x.Estado);
        builder.HasIndex(x => x.ActivoId);
        builder.HasQueryFilter(x => !x.IsDeleted);

        builder.HasMany(x => x.Fotos)
            .WithOne()
            .HasForeignKey(f => f.IncidenciaId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(x => x.Comentarios)
            .WithOne()
            .HasForeignKey(c => c.IncidenciaId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

public class FotoIncidenciaConfiguration : IEntityTypeConfiguration<FotoIncidencia>
{
    public void Configure(EntityTypeBuilder<FotoIncidencia> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasConversion(id => id.Value, v => new FotoIncidenciaId(v));
        builder.Property(x => x.IncidenciaId).HasConversion(id => id.Value, v => new IncidenciaId(v));
        builder.Property(x => x.Url).HasMaxLength(500).IsRequired();
        builder.Property(x => x.Descripcion).HasMaxLength(500);
    }
}

public class ComentarioIncidenciaConfiguration : IEntityTypeConfiguration<ComentarioIncidencia>
{
    public void Configure(EntityTypeBuilder<ComentarioIncidencia> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasConversion(id => id.Value, v => new ComentarioIncidenciaId(v));
        builder.Property(x => x.IncidenciaId).HasConversion(id => id.Value, v => new IncidenciaId(v));
        builder.Property(x => x.Texto).HasMaxLength(2000).IsRequired();
    }
}
