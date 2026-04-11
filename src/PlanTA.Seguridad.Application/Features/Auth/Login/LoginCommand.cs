using PlanTA.SharedKernel.CQRS;
using PlanTA.Seguridad.Application.DTOs;

namespace PlanTA.Seguridad.Application.Features.Auth.Login;

public record LoginCommand(string Email, string Password) : ICommand<TokenPairDto>;
