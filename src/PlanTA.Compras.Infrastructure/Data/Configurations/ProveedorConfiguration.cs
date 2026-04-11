using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PlanTA.Compras.Domain.Entities;

namespace PlanTA.Compras.Infrastructure.Data.Configurations;

public class ProveedorConfiguration : IEntityTypeConfiguration<Proveedor>
{
    public void Configure(EntityTypeBuilder<Proveedor> builder)
    {
        builder.ToTable("proveedores");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasConversion(id => id.Value, v => new ProveedorId(v));

        builder.Property(x => x.RazonSocial).HasMaxLength(300).IsRequired();
        builder.Property(x => x.NIF).HasMaxLength(20).IsRequired();
        builder.Property(x => x.Direccion).HasMaxLength(500);
        builder.Property(x => x.Ciudad).HasMaxLength(100);
        builder.Property(x => x.CodigoPostal).HasMaxLength(10);
        builder.Property(x => x.Pais).HasMaxLength(100);
        builder.Property(x => x.Email).HasMaxLength(200).IsRequired();
        builder.Property(x => x.Telefono).HasMaxLength(50);
        builder.Property(x => x.Web).HasMaxLength(300);

        builder.OwnsOne(x => x.CondicionesPago, cp =>
        {
            cp.Property(c => c.DiasVencimiento).HasColumnName("DiasVencimiento");
            cp.Property(c => c.DescuentoProntoPago).HasColumnName("DescuentoProntoPago").HasPrecision(5, 2);
            cp.Property(c => c.MetodoPago).HasColumnName("MetodoPago").HasMaxLength(100).IsRequired();
        });

        builder.HasMany(x => x.Contactos)
            .WithOne()
            .HasForeignKey(c => c.ProveedorId);

        builder.HasIndex(x => new { x.NIF, x.EmpresaId }).IsUnique();
        builder.HasIndex(x => x.EmpresaId);
        builder.HasQueryFilter(x => !x.IsDeleted);
    }
}
