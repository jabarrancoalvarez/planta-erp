using System.IO.Compression;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PlanTA.CRM.Application.Interfaces;
using PlanTA.Facturacion.Application.Interfaces;
using PlanTA.RRHH.Application.Interfaces;
using PlanTA.Seguridad.Domain.Entities;
using PlanTA.Seguridad.Infrastructure.Data;
using PlanTA.Seguridad.Infrastructure.Identity;
using PlanTA.SharedKernel;

namespace PlanTA.API.Endpoints;

public sealed class GdprEndpoints : IEndpointGroup
{
    public string? RoutePrefix => "/api/v1/gdpr";

    private static readonly JsonSerializerOptions Json = new()
    {
        WriteIndented = true,
        DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull,
    };

    public void MapEndpoints(RouteGroupBuilder group)
    {
        group.MapGet("/export", async (
            ICurrentTenant tenant,
            UserManager<ApplicationUser> userManager,
            SeguridadDbContext seguridad,
            IFacturacionDbContext facturacion,
            IRRHHDbContext rrhh,
            ICrmDbContext crm,
            CancellationToken ct) =>
        {
            var user = await userManager.FindByIdAsync(tenant.UserId.ToString());
            if (user is null) return Results.Unauthorized();

            var empresaId = new EmpresaId(tenant.EmpresaId);
            var empresa = await seguridad.Empresas.AsNoTracking()
                .Where(e => e.Id == empresaId)
                .FirstOrDefaultAsync(ct);

            var facturas = await facturacion.Facturas.AsNoTracking()
                .Where(f => f.EmpresaId == tenant.EmpresaId)
                .OrderByDescending(f => f.FechaEmision)
                .Take(500)
                .Select(f => new { f.NumeroCompleto, f.ClienteNombre, f.FechaEmision, f.Total, f.Estado })
                .ToListAsync(ct);

            var empleados = await rrhh.Empleados.AsNoTracking()
                .Where(e => e.EmpresaId == tenant.EmpresaId)
                .Select(e => new { e.Nombre, e.Apellidos, e.Email, e.Puesto, e.Departamento })
                .ToListAsync(ct);

            var fichajes = await rrhh.Fichajes.AsNoTracking()
                .Where(f => f.EmpresaId == tenant.EmpresaId)
                .OrderByDescending(f => f.Momento)
                .Take(2000)
                .Select(f => new { f.EmpleadoId, f.Tipo, f.Momento, f.Notas })
                .ToListAsync(ct);

            var clientes = await crm.Leads.AsNoTracking()
                .Where(l => l.EmpresaId == tenant.EmpresaId)
                .Select(l => new { l.Nombre, l.Email, l.Telefono, l.Estado })
                .ToListAsync(ct);

            using var ms = new MemoryStream();
            using (var zip = new ZipArchive(ms, ZipArchiveMode.Create, leaveOpen: true))
            {
                WriteJsonEntry(zip, "user.json", new
                {
                    user.Id,
                    user.Email,
                    user.Nombre,
                    user.CreatedAt,
                    Roles = await userManager.GetRolesAsync(user),
                    Empresa = empresa is null ? null : new { empresa.Nombre, empresa.CIF, empresa.TrialHasta, empresa.OnboardingCompletado },
                });
                WriteJsonEntry(zip, "facturas.json", facturas);
                WriteJsonEntry(zip, "empleados.json", empleados);
                WriteJsonEntry(zip, "fichajes.json", fichajes);
                WriteJsonEntry(zip, "leads.json", clientes);
                WriteTextEntry(zip, "README.txt",
                    "Exportación de datos PlanTA — RGPD Art. 20\n" +
                    $"Generado: {DateTimeOffset.UtcNow:O}\n" +
                    $"Usuario: {user.Email}\n" +
                    "Contenido: datos personales y operativos asociados a tu empresa.\n");
            }

            return Results.File(
                ms.ToArray(),
                "application/zip",
                $"planta-export-{DateTime.UtcNow:yyyyMMdd-HHmmss}.zip");
        }).WithName("GdprExport").WithTags("GDPR");

        group.MapDelete("/account", async (
            ICurrentTenant tenant,
            UserManager<ApplicationUser> userManager,
            SeguridadDbContext seguridad,
            CancellationToken ct) =>
        {
            var user = await userManager.FindByIdAsync(tenant.UserId.ToString());
            if (user is null) return Results.Unauthorized();

            var isAdmin = (await userManager.GetRolesAsync(user)).Contains("Administrador");
            if (isAdmin)
            {
                var empresaId = new EmpresaId(tenant.EmpresaId);
                var empresa = await seguridad.Empresas
                    .FirstOrDefaultAsync(e => e.Id == empresaId, ct);
                if (empresa is not null)
                {
                    empresa.SoftDelete();
                }
            }

            var result = await userManager.DeleteAsync(user);
            if (!result.Succeeded)
                return Results.BadRequest(new { errors = result.Errors.Select(e => e.Description) });

            await seguridad.SaveChangesAsync(ct);
            return Results.Ok(new { message = "Cuenta eliminada. Los datos se conservan en estado de borrado lógico 30 días y después se purgan." });
        }).WithName("GdprDeleteAccount").WithTags("GDPR");
    }

    private static void WriteJsonEntry(ZipArchive zip, string name, object payload)
    {
        var entry = zip.CreateEntry(name, CompressionLevel.Optimal);
        using var stream = entry.Open();
        JsonSerializer.Serialize(stream, payload, Json);
    }

    private static void WriteTextEntry(ZipArchive zip, string name, string content)
    {
        var entry = zip.CreateEntry(name, CompressionLevel.Optimal);
        using var stream = entry.Open();
        var bytes = Encoding.UTF8.GetBytes(content);
        stream.Write(bytes, 0, bytes.Length);
    }
}
