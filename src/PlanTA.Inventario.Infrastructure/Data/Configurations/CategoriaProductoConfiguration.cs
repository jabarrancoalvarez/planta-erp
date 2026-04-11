using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PlanTA.Inventario.Domain.Entities;

namespace PlanTA.Inventario.Infrastructure.Data.Configurations;

public class CategoriaProductoConfiguration : IEntityTypeConfiguration<CategoriaProducto>
{
    public void Configure(EntityTypeBuilder<CategoriaProducto> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasConversion(id => id.Value, v => new CategoriaProductoId(v));

        builder.Property(x => x.Nombre).HasMaxLength(200).IsRequired();
        builder.Property(x => x.Descripcion).HasMaxLength(500);

        builder.Property(x => x.CategoriaPadreId)
            .HasConversion(id => id != null ? id.Value : (Guid?)null, v => v.HasValue ? new CategoriaProductoId(v.Value) : null);

        builder.HasIndex(x => x.EmpresaId);
        builder.HasQueryFilter(x => !x.IsDeleted);
    }
}
