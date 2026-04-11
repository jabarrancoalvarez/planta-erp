namespace PlanTA.SharedKernel;

/// <summary>
/// Inyectado desde el middleware de autenticación.
/// Todos los queries deben filtrar por EmpresaId.
/// </summary>
public interface ICurrentTenant
{
    Guid EmpresaId { get; }
    Guid UserId { get; }
    string Role { get; }
}
