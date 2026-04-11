using MediatR;
using PlanTA.SharedKernel;
using PlanTA.Seguridad.Application.DTOs;
using PlanTA.Seguridad.Application.Interfaces;

namespace PlanTA.Seguridad.Application.Features.Auth.Refresh;

public sealed class RefreshCommandHandler(IIdentityService identityService)
    : IRequestHandler<RefreshCommand, Result<TokenPairDto>>
{
    public async Task<Result<TokenPairDto>> Handle(RefreshCommand request, CancellationToken cancellationToken)
    {
        return await identityService.RefreshAsync(request.RefreshToken);
    }
}
