using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PlanTA.RRHH.Domain.Entities;

namespace PlanTA.RRHH.Infrastructure.Data.Configurations;

public class TurnoConfiguration : IEntityTypeConfiguration<Turno>
{
    public void Configure(EntityTypeBuilder<Turno> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasConversion(id => id.Value, v => new TurnoId(v));

        builder.Property(x => x.Nombre).HasMaxLength(100).IsRequired();
        builder.Property(x => x.DiasSemana).HasMaxLength(10);

        builder.HasIndex(x => x.EmpresaId);
        builder.HasQueryFilter(x => !x.IsDeleted);
    }
}
