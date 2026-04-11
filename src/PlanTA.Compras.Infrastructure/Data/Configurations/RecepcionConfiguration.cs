using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PlanTA.Compras.Domain.Entities;

namespace PlanTA.Compras.Infrastructure.Data.Configurations;

public class RecepcionConfiguration : IEntityTypeConfiguration<Recepcion>
{
    public void Configure(EntityTypeBuilder<Recepcion> builder)
    {
        builder.ToTable("recepciones");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasConversion(id => id.Value, v => new RecepcionId(v));

        builder.Property(x => x.OrdenCompraId)
            .HasConversion(id => id.Value, v => new OrdenCompraId(v));

        builder.Property(x => x.NumeroAlbaran).HasMaxLength(100);
        builder.Property(x => x.Observaciones).HasMaxLength(2000);

        builder.HasMany(x => x.Lineas)
            .WithOne()
            .HasForeignKey(l => l.RecepcionId);

        builder.HasIndex(x => x.EmpresaId);
        builder.HasIndex(x => x.OrdenCompraId);
        builder.HasIndex(x => x.EstadoRecepcion);
    }
}
