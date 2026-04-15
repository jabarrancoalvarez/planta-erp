namespace PlanTA.Seguridad.Application.DTOs;

public record TokenPairDto(string AccessToken, string RefreshToken, UserDto User);

public record UserDto(
    Guid Id,
    string Email,
    string Nombre,
    string Rol,
    Guid EmpresaId,
    string EmpresaNombre,
    bool OnboardingCompletado = false,
    DateTimeOffset? TrialHasta = null,
    string[]? ModulosDeshabilitados = null);
