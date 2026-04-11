using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PlanTA.Produccion.Domain.Entities;

namespace PlanTA.Produccion.Infrastructure.Data.Configurations;

public class ListaMaterialesConfiguration : IEntityTypeConfiguration<ListaMateriales>
{
    public void Configure(EntityTypeBuilder<ListaMateriales> builder)
    {
        builder.ToTable("listas_materiales");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasConversion(id => id.Value, v => new ListaMaterialesId(v));

        builder.Property(x => x.Nombre).HasMaxLength(200).IsRequired();
        builder.Property(x => x.Descripcion).HasMaxLength(1000);

        builder.HasMany(x => x.Lineas)
            .WithOne()
            .HasForeignKey(l => l.ListaMaterialesId);

        builder.HasIndex(x => x.EmpresaId);
        builder.HasIndex(x => x.ProductoId);
        builder.HasQueryFilter(x => !x.IsDeleted);
    }
}
