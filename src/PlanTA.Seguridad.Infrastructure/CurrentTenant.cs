using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using PlanTA.SharedKernel;

namespace PlanTA.Seguridad.Infrastructure;

public class CurrentTenant : ICurrentTenant
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentTenant(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    private ClaimsPrincipal? User => _httpContextAccessor.HttpContext?.User;

    public Guid EmpresaId
    {
        get
        {
            var claim = User?.FindFirst("empresaId")?.Value;
            return Guid.TryParse(claim, out var id) ? id : Guid.Empty;
        }
    }

    public Guid UserId
    {
        get
        {
            var claim = User?.FindFirst(ClaimTypes.NameIdentifier)?.Value
                     ?? User?.FindFirst("sub")?.Value;
            return Guid.TryParse(claim, out var id) ? id : Guid.Empty;
        }
    }

    public string Role => User?.FindFirst(ClaimTypes.Role)?.Value ?? string.Empty;
}
