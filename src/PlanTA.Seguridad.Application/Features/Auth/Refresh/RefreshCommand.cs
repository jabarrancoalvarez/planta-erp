using PlanTA.SharedKernel.CQRS;
using PlanTA.Seguridad.Application.DTOs;

namespace PlanTA.Seguridad.Application.Features.Auth.Refresh;

public record RefreshCommand(string RefreshToken) : ICommand<TokenPairDto>;
