using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using PlanTA.Seguridad.Application.DTOs;
using PlanTA.Seguridad.Application.Interfaces;
using PlanTA.Seguridad.Infrastructure.Data;
using PlanTA.SharedKernel;

namespace PlanTA.Seguridad.Infrastructure.Identity;

public sealed class IdentityService(
    UserManager<ApplicationUser> userManager,
    SeguridadDbContext db,
    IConfiguration configuration) : IIdentityService
{
    public async Task<Result<Guid>> CreateUserAsync(
        string email, string password, string nombre, string rol, Guid empresaId)
    {
        var user = new ApplicationUser
        {
            UserName = email,
            Email = email,
            Nombre = nombre,
            EmpresaId = empresaId
        };

        var result = await userManager.CreateAsync(user, password);
        if (!result.Succeeded)
        {
            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            return Result<Guid>.Failure("User.CreationFailed", errors);
        }

        await userManager.AddToRoleAsync(user, rol);
        return Result<Guid>.Success(user.Id);
    }

    public async Task<Result<TokenPairDto>> LoginAsync(string email, string password)
    {
        var user = await userManager.FindByEmailAsync(email);
        if (user is null || !await userManager.CheckPasswordAsync(user, password))
            return Result<TokenPairDto>.Failure(
                Error.Validation("Auth.InvalidCredentials", "Email o contraseña incorrectos"));

        var roles = await userManager.GetRolesAsync(user);
        var rol = roles.FirstOrDefault() ?? "Operario";

        var empresaId = new PlanTA.Seguridad.Domain.Entities.EmpresaId(user.EmpresaId);
        var empresa = await db.Empresas.FirstOrDefaultAsync(e => e.Id == empresaId);

        var accessToken = GenerateAccessToken(user, rol);
        var refreshToken = GenerateRefreshToken();

        user.RefreshToken = refreshToken;
        user.RefreshTokenExpiryTime = DateTimeOffset.UtcNow.AddDays(
            configuration.GetValue<int>("Jwt:RefreshTokenExpirationDays", 7));
        await userManager.UpdateAsync(user);

        var userDto = new UserDto(user.Id, user.Email!, user.Nombre, rol,
            user.EmpresaId, empresa?.Nombre ?? "");

        return Result<TokenPairDto>.Success(new TokenPairDto(accessToken, refreshToken, userDto));
    }

    public async Task<Result<TokenPairDto>> RefreshAsync(string refreshToken)
    {
        var user = await db.Users.FirstOrDefaultAsync(u => u.RefreshToken == refreshToken);
        if (user is null || user.RefreshTokenExpiryTime <= DateTimeOffset.UtcNow)
            return Result<TokenPairDto>.Failure(
                Error.Validation("Auth.InvalidRefreshToken", "Refresh token inv��lido o expirado"));

        var roles = await userManager.GetRolesAsync(user);
        var rol = roles.FirstOrDefault() ?? "Operario";
        var empresaId = new PlanTA.Seguridad.Domain.Entities.EmpresaId(user.EmpresaId);
        var empresa = await db.Empresas.FirstOrDefaultAsync(e => e.Id == empresaId);

        var newAccessToken = GenerateAccessToken(user, rol);
        var newRefreshToken = GenerateRefreshToken();

        user.RefreshToken = newRefreshToken;
        user.RefreshTokenExpiryTime = DateTimeOffset.UtcNow.AddDays(
            configuration.GetValue<int>("Jwt:RefreshTokenExpirationDays", 7));
        await userManager.UpdateAsync(user);

        var userDto = new UserDto(user.Id, user.Email!, user.Nombre, rol,
            user.EmpresaId, empresa?.Nombre ?? "");

        return Result<TokenPairDto>.Success(new TokenPairDto(newAccessToken, newRefreshToken, userDto));
    }

    public async Task<Result<bool>> ChangePasswordAsync(Guid userId, string currentPassword, string newPassword)
    {
        var user = await userManager.FindByIdAsync(userId.ToString());
        if (user is null)
            return Result<bool>.Failure(Error.NotFound("User.NotFound", "Usuario no encontrado"));

        var result = await userManager.ChangePasswordAsync(user, currentPassword, newPassword);
        return result.Succeeded
            ? Result<bool>.Success(true)
            : Result<bool>.Failure("User.PasswordChangeFailed",
                string.Join(", ", result.Errors.Select(e => e.Description)));
    }

    public async Task<Result<UserDto>> GetUserByIdAsync(Guid userId)
    {
        var user = await userManager.FindByIdAsync(userId.ToString());
        if (user is null)
            return Result<UserDto>.Failure(Error.NotFound("User.NotFound", "Usuario no encontrado"));

        var roles = await userManager.GetRolesAsync(user);
        var rol = roles.FirstOrDefault() ?? "Operario";
        var empresaId = new PlanTA.Seguridad.Domain.Entities.EmpresaId(user.EmpresaId);
        var empresa = await db.Empresas.FirstOrDefaultAsync(e => e.Id == empresaId);

        return Result<UserDto>.Success(new UserDto(
            user.Id, user.Email!, user.Nombre, rol, user.EmpresaId, empresa?.Nombre ?? ""));
    }

    private string GenerateAccessToken(ApplicationUser user, string role)
    {
        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(configuration["Jwt:Key"]!));

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email!),
            new Claim("nombre", user.Nombre),
            new Claim("empresaId", user.EmpresaId.ToString()),
            new Claim(ClaimTypes.Role, role),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var expirationMinutes = configuration.GetValue<int>("Jwt:AccessTokenExpirationMinutes", 15);

        var token = new JwtSecurityToken(
            issuer: configuration["Jwt:Issuer"],
            audience: configuration["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(expirationMinutes),
            signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256));

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private static string GenerateRefreshToken()
    {
        var randomNumber = new byte[64];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        return Convert.ToBase64String(randomNumber);
    }
}
