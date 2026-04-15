using PlanTA.SharedKernel;
using PlanTA.Seguridad.Application.DTOs;

namespace PlanTA.Seguridad.Application.Interfaces;

public interface IIdentityService
{
    Task<Result<Guid>> CreateUserAsync(string email, string password, string nombre, string rol, Guid empresaId);
    Task<Result<TokenPairDto>> RegisterEmpresaAsync(string nombreEmpresa, string? cif, string email, string password, string nombreAdmin);
    Task<Result<TokenPairDto>> LoginAsync(string email, string password);
    Task<Result<TokenPairDto>> RefreshAsync(string refreshToken);
    Task<Result<bool>> ChangePasswordAsync(Guid userId, string currentPassword, string newPassword);
    Task<Result<UserDto>> GetUserByIdAsync(Guid userId);
}
