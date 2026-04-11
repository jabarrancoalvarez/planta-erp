using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PlanTA.Compras.Domain.Entities;

namespace PlanTA.Compras.Infrastructure.Data.Configurations;

public class ContactoProveedorConfiguration : IEntityTypeConfiguration<ContactoProveedor>
{
    public void Configure(EntityTypeBuilder<ContactoProveedor> builder)
    {
        builder.ToTable("contactos_proveedor");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasConversion(id => id.Value, v => new ContactoProveedorId(v));

        builder.Property(x => x.ProveedorId)
            .HasConversion(id => id.Value, v => new ProveedorId(v));

        builder.Property(x => x.Nombre).HasMaxLength(200).IsRequired();
        builder.Property(x => x.Cargo).HasMaxLength(100);
        builder.Property(x => x.Email).HasMaxLength(200).IsRequired();
        builder.Property(x => x.Telefono).HasMaxLength(50);
    }
}
