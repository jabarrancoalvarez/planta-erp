using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PlanTA.RRHH.Domain.Entities;

namespace PlanTA.RRHH.Infrastructure.Data.Configurations;

public class FichajeConfiguration : IEntityTypeConfiguration<Fichaje>
{
    public void Configure(EntityTypeBuilder<Fichaje> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasConversion(id => id.Value, v => new FichajeId(v));

        builder.Property(x => x.EmpleadoId).HasConversion(id => id.Value, v => new EmpleadoId(v));
        builder.Property(x => x.Notas).HasMaxLength(500);

        builder.HasIndex(x => new { x.EmpresaId, x.EmpleadoId, x.Momento });
    }
}
