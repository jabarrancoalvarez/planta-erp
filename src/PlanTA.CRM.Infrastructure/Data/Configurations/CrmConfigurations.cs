using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PlanTA.CRM.Domain.Entities;

namespace PlanTA.CRM.Infrastructure.Data.Configurations;

public class LeadConfiguration : IEntityTypeConfiguration<Lead>
{
    public void Configure(EntityTypeBuilder<Lead> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasConversion(id => id.Value, v => new LeadId(v));
        builder.Property(x => x.Nombre).HasMaxLength(200).IsRequired();
        builder.Property(x => x.Empresa).HasMaxLength(200);
        builder.Property(x => x.Email).HasMaxLength(200);
        builder.Property(x => x.Telefono).HasMaxLength(30);
        builder.Property(x => x.Notas).HasMaxLength(2000);
        builder.HasIndex(x => new { x.EmpresaId, x.Estado });
        builder.HasQueryFilter(x => !x.IsDeleted);
    }
}

public class OportunidadConfiguration : IEntityTypeConfiguration<Oportunidad>
{
    public void Configure(EntityTypeBuilder<Oportunidad> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasConversion(id => id.Value, v => new OportunidadId(v));
        builder.Property(x => x.Titulo).HasMaxLength(200).IsRequired();
        builder.Property(x => x.Descripcion).HasMaxLength(2000);
        builder.Property(x => x.ImporteEstimado).HasPrecision(18, 4);
        builder.HasIndex(x => new { x.EmpresaId, x.Fase });
        builder.HasQueryFilter(x => !x.IsDeleted);
    }
}

public class ActividadCrmConfiguration : IEntityTypeConfiguration<ActividadCrm>
{
    public void Configure(EntityTypeBuilder<ActividadCrm> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasConversion(id => id.Value, v => new ActividadCrmId(v));
        builder.Property(x => x.Asunto).HasMaxLength(300).IsRequired();
        builder.Property(x => x.Detalle).HasMaxLength(4000);
        builder.HasIndex(x => new { x.EmpresaId, x.Fecha });
    }
}
