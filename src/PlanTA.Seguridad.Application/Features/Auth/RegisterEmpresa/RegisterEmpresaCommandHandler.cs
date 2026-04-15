using MediatR;
using PlanTA.Seguridad.Application.DTOs;
using PlanTA.Seguridad.Application.Interfaces;
using PlanTA.SharedKernel;

namespace PlanTA.Seguridad.Application.Features.Auth.RegisterEmpresa;

public sealed class RegisterEmpresaCommandHandler(IIdentityService identityService)
    : IRequestHandler<RegisterEmpresaCommand, Result<TokenPairDto>>
{
    public async Task<Result<TokenPairDto>> Handle(RegisterEmpresaCommand request, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(request.NombreEmpresa))
            return Result<TokenPairDto>.Failure(Error.Validation("Registro.NombreEmpresaVacio", "Nombre de empresa obligatorio"));
        if (string.IsNullOrWhiteSpace(request.EmailAdmin))
            return Result<TokenPairDto>.Failure(Error.Validation("Registro.EmailVacio", "Email obligatorio"));
        if (string.IsNullOrWhiteSpace(request.Password) || request.Password.Length < 8)
            return Result<TokenPairDto>.Failure(Error.Validation("Registro.PasswordInvalida", "Password mínimo 8 caracteres"));
        if (string.IsNullOrWhiteSpace(request.NombreAdmin))
            return Result<TokenPairDto>.Failure(Error.Validation("Registro.NombreAdminVacio", "Nombre del administrador obligatorio"));

        return await identityService.RegisterEmpresaAsync(
            request.NombreEmpresa.Trim(),
            request.Cif,
            request.EmailAdmin.Trim().ToLowerInvariant(),
            request.Password,
            request.NombreAdmin.Trim());
    }
}
