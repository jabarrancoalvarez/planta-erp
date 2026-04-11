using PlanTA.SharedKernel;

namespace PlanTA.Seguridad.Infrastructure;

public class CurrentTenant : ICurrentTenant
{
    public Guid EmpresaId { get; set; }
    public Guid UserId { get; set; }
    public string Role { get; set; } = string.Empty;
}
