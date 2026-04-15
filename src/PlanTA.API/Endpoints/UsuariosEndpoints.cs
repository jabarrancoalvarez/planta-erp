using Microsoft.AspNetCore.Authorization;
using PlanTA.Seguridad.Application.Interfaces;
using PlanTA.SharedKernel;
using PlanTA.SharedKernel.Extensions;

namespace PlanTA.API.Endpoints;

public sealed class UsuariosEndpoints : IEndpointGroup
{
    public string? RoutePrefix => "/api/v1/seguridad/usuarios";

    public void MapEndpoints(RouteGroupBuilder group)
    {
        group.MapGet("/", [Authorize(Roles = "Administrador")] async (
            IIdentityService identity, ICurrentTenant tenant) =>
        {
            var result = await identity.ListUsersByEmpresaAsync(tenant.EmpresaId);
            return result.ToHttpResult();
        })
        .WithName("ListarUsuariosEmpresa")
        .WithTags("Usuarios");

        group.MapPut("/{userId:guid}/modulos", [Authorize(Roles = "Administrador")] async (
            Guid userId, UpdateModulosRequest req, IIdentityService identity) =>
        {
            var result = await identity.UpdateModulosDeshabilitadosAsync(userId, req.ModulosDeshabilitados ?? Array.Empty<string>());
            return result.ToHttpResult();
        })
        .WithName("ActualizarModulosUsuario")
        .WithTags("Usuarios");
    }
}

public record UpdateModulosRequest(string[] ModulosDeshabilitados);
