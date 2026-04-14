using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PlanTA.RRHH.Domain.Entities;

namespace PlanTA.RRHH.Infrastructure.Data.Configurations;

public class AusenciaConfiguration : IEntityTypeConfiguration<Ausencia>
{
    public void Configure(EntityTypeBuilder<Ausencia> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasConversion(id => id.Value, v => new AusenciaId(v));

        builder.Property(x => x.EmpleadoId).HasConversion(id => id.Value, v => new EmpleadoId(v));
        builder.Property(x => x.Motivo).HasMaxLength(1000);

        builder.HasIndex(x => new { x.EmpresaId, x.EmpleadoId, x.FechaInicio });
        builder.HasQueryFilter(x => !x.IsDeleted);
    }
}
