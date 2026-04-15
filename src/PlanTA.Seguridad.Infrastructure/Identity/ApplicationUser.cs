using Microsoft.AspNetCore.Identity;

namespace PlanTA.Seguridad.Infrastructure.Identity;

public class ApplicationUser : IdentityUser<Guid>
{
    public string Nombre { get; set; } = string.Empty;
    public Guid EmpresaId { get; set; }
    public string? RefreshToken { get; set; }
    public DateTimeOffset? RefreshTokenExpiryTime { get; set; }
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
    public string? ModulosDeshabilitados { get; set; }
}
