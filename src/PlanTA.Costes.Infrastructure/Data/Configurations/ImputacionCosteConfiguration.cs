using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PlanTA.Costes.Domain.Entities;

namespace PlanTA.Costes.Infrastructure.Data.Configurations;

public class ImputacionCosteConfiguration : IEntityTypeConfiguration<ImputacionCoste>
{
    public void Configure(EntityTypeBuilder<ImputacionCoste> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasConversion(id => id.Value, v => new ImputacionCosteId(v));

        builder.Property(x => x.Cantidad).HasPrecision(18, 4);
        builder.Property(x => x.PrecioUnitario).HasPrecision(18, 4);
        builder.Property(x => x.Importe).HasPrecision(18, 4);
        builder.Property(x => x.Concepto).HasMaxLength(500);

        builder.HasIndex(x => new { x.EmpresaId, x.OrdenFabricacionId });
        builder.HasIndex(x => new { x.EmpresaId, x.OrdenTrabajoId });
        builder.HasIndex(x => new { x.EmpresaId, x.Fecha });
    }
}
