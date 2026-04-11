using MediatR;
using PlanTA.SharedKernel;
using PlanTA.Seguridad.Application.DTOs;
using PlanTA.Seguridad.Application.Interfaces;

namespace PlanTA.Seguridad.Application.Features.Auth.Login;

public sealed class LoginCommandHandler(IIdentityService identityService)
    : IRequestHandler<LoginCommand, Result<TokenPairDto>>
{
    public async Task<Result<TokenPairDto>> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        return await identityService.LoginAsync(request.Email, request.Password);
    }
}
