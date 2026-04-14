using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PlanTA.RRHH.Domain.Entities;

namespace PlanTA.RRHH.Infrastructure.Data.Configurations;

public class EmpleadoConfiguration : IEntityTypeConfiguration<Empleado>
{
    public void Configure(EntityTypeBuilder<Empleado> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasConversion(id => id.Value, v => new EmpleadoId(v));

        builder.Property(x => x.Codigo).HasMaxLength(30).IsRequired();
        builder.Property(x => x.Nombre).HasMaxLength(100).IsRequired();
        builder.Property(x => x.Apellidos).HasMaxLength(150).IsRequired();
        builder.Property(x => x.DNI).HasMaxLength(20).IsRequired();
        builder.Property(x => x.Email).HasMaxLength(200);
        builder.Property(x => x.Telefono).HasMaxLength(30);
        builder.Property(x => x.Puesto).HasMaxLength(100).IsRequired();
        builder.Property(x => x.Departamento).HasMaxLength(100);
        builder.Property(x => x.CosteHoraEstandar).HasPrecision(18, 4);

        builder.HasIndex(x => new { x.EmpresaId, x.DNI }).IsUnique();
        builder.HasIndex(x => new { x.EmpresaId, x.Codigo }).IsUnique();
        builder.HasQueryFilter(x => !x.IsDeleted);
    }
}
