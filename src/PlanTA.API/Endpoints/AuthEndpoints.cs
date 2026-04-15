using MediatR;
using PlanTA.Seguridad.Application.Features.Auth.Login;
using PlanTA.Seguridad.Application.Features.Auth.Refresh;
using PlanTA.Seguridad.Application.Features.Auth.RegisterEmpresa;
using PlanTA.SharedKernel.Extensions;

namespace PlanTA.API.Endpoints;

public sealed class AuthEndpoints : IEndpointGroup
{
    public string? RoutePrefix => "/api/v1/seguridad/auth";

    public void MapEndpoints(RouteGroupBuilder group)
    {
        group.MapPost("/login", async (LoginRequest req, IMediator m, CancellationToken ct) =>
        {
            var result = await m.Send(new LoginCommand(req.Email, req.Password), ct);
            return result.ToHttpResult();
        })
        .WithName("Login")
        .WithTags("Auth")
        .AllowAnonymous();

        group.MapPost("/register", async (RegisterEmpresaRequest req, IMediator m, CancellationToken ct) =>
        {
            var result = await m.Send(new RegisterEmpresaCommand(
                req.EmpresaNombre, req.Cif, req.Email, req.Password, req.Nombre), ct);
            return result.ToHttpResult();
        })
        .WithName("RegisterEmpresa")
        .WithTags("Auth")
        .AllowAnonymous();

        group.MapPost("/refresh", async (RefreshRequest req, IMediator m, CancellationToken ct) =>
        {
            var result = await m.Send(new RefreshCommand(req.RefreshToken), ct);
            return result.ToHttpResult();
        })
        .WithName("Refresh")
        .WithTags("Auth")
        .AllowAnonymous();
    }
}

public record LoginRequest(string Email, string Password);
public record RefreshRequest(string RefreshToken);
public record RegisterEmpresaRequest(string Nombre, string Email, string Password, string EmpresaNombre, string? Cif = null);
