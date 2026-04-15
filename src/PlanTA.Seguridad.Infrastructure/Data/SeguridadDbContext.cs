using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PlanTA.Seguridad.Domain.Entities;
using PlanTA.Seguridad.Infrastructure.Identity;

namespace PlanTA.Seguridad.Infrastructure.Data;

public class SeguridadDbContext : IdentityDbContext<ApplicationUser, IdentityRole<Guid>, Guid>
{
    public SeguridadDbContext(DbContextOptions<SeguridadDbContext> options) : base(options) { }

    public DbSet<Empresa> Empresas => Set<Empresa>();
    public DbSet<Notificacion> Notificaciones => Set<Notificacion>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.HasDefaultSchema("seguridad");

        builder.Entity<Empresa>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.Id).HasConversion(id => id.Value, v => new EmpresaId(v));
            e.Property(x => x.Nombre).HasMaxLength(200).IsRequired();
            e.Property(x => x.CIF).HasMaxLength(20);
            e.Property(x => x.Email).HasMaxLength(200);
            e.HasQueryFilter(x => !x.IsDeleted);
        });

        builder.Entity<ApplicationUser>(e =>
        {
            e.Property(u => u.Nombre).HasMaxLength(200).IsRequired();
            e.HasIndex(u => u.EmpresaId);
        });

        builder.Entity<Notificacion>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.Titulo).HasMaxLength(200).IsRequired();
            e.Property(x => x.Mensaje).HasMaxLength(2000).IsRequired();
            e.Property(x => x.Tipo).HasMaxLength(20).IsRequired();
            e.Property(x => x.Url).HasMaxLength(500);
            e.HasIndex(x => new { x.EmpresaId, x.UsuarioId, x.Leida });
        });
    }
}
