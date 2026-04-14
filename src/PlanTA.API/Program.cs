using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PlanTA.API.Endpoints;
using PlanTA.API.Infrastructure;
using PlanTA.API.Middleware;
using PlanTA.Activos.Infrastructure;
using PlanTA.Activos.Infrastructure.Data;
using PlanTA.Calidad.Infrastructure.Data;
using PlanTA.Compras.Infrastructure.Data;
using PlanTA.Incidencias.Infrastructure;
using PlanTA.Incidencias.Infrastructure.Data;
using PlanTA.RRHH.Infrastructure;
using PlanTA.RRHH.Infrastructure.Data;
using PlanTA.Costes.Infrastructure;
using PlanTA.Costes.Infrastructure.Data;
using PlanTA.Importador.Infrastructure;
using PlanTA.Importador.Infrastructure.Data;
using PlanTA.CRM.Infrastructure;
using PlanTA.CRM.Infrastructure.Data;
using PlanTA.Facturacion.Infrastructure;
using PlanTA.Facturacion.Infrastructure.Data;
using PlanTA.OEE.Infrastructure;
using PlanTA.OEE.Infrastructure.Data;
using PlanTA.IA.Infrastructure;
using PlanTA.IA.Infrastructure.Data;
using PlanTA.Inventario.Infrastructure.Data;
using PlanTA.Mantenimiento.Infrastructure;
using PlanTA.Mantenimiento.Infrastructure.Data;
using PlanTA.Produccion.Infrastructure.Data;
using PlanTA.Seguridad.Domain.Entities;
using PlanTA.Seguridad.Infrastructure.Data;
using PlanTA.Seguridad.Infrastructure.Identity;
using PlanTA.Ventas.Infrastructure.Data;
using PlanTA.Calidad.Infrastructure;
using PlanTA.Compras.Infrastructure;
using PlanTA.Inventario.Infrastructure;
using PlanTA.Produccion.Infrastructure;
using PlanTA.Seguridad.Infrastructure;
using PlanTA.SharedKernel.Audit;
using PlanTA.SharedKernel.Behaviors;
using PlanTA.SharedKernel.Outbox;
using PlanTA.Ventas.Infrastructure;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// -- Serilog --
builder.Host.UseSerilog((context, config) =>
    config.ReadFrom.Configuration(context.Configuration));

// -- MediatR + Behaviors --
builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssemblies(
        typeof(PlanTA.Seguridad.Application.Features.Auth.Login.LoginCommand).Assembly,
        typeof(PlanTA.Inventario.Application.Features.Alertas.CreateAlerta.CreateAlertaCommand).Assembly,
        typeof(PlanTA.Produccion.Application.Features.BOM.AddLineaBOM.AddLineaBOMCommand).Assembly,
        typeof(PlanTA.Compras.Application.Features.OrdenesCompra.AddLineaOC.AddLineaOCCommand).Assembly,
        typeof(PlanTA.Ventas.Application.Features.Clientes.CreateCliente.CreateClienteCommand).Assembly,
        typeof(PlanTA.Calidad.Application.Features.Inspecciones.CompletarInspeccion.CompletarInspeccionCommand).Assembly,
        typeof(PlanTA.Activos.Application.Features.Activos.CreateActivo.CreateActivoCommand).Assembly,
        typeof(PlanTA.Mantenimiento.Application.Features.OrdenesTrabajo.CreateOrdenTrabajo.CreateOrdenTrabajoCommand).Assembly,
        typeof(PlanTA.Incidencias.Application.Features.Incidencias.CreateIncidencia.CreateIncidenciaCommand).Assembly,
        typeof(PlanTA.RRHH.Application.Features.Empleados.CreateEmpleado.CreateEmpleadoCommand).Assembly,
        typeof(PlanTA.Costes.Application.Features.Imputaciones.CreateImputacion.CreateImputacionCommand).Assembly,
        typeof(PlanTA.Importador.Application.Features.Jobs.CreateJob.CreateImportJobCommand).Assembly,
        typeof(PlanTA.CRM.Application.Features.Leads.CreateLead.CreateLeadCommand).Assembly,
        typeof(PlanTA.Facturacion.Application.Features.Facturas.CreateFactura.CreateFacturaCommand).Assembly,
        typeof(PlanTA.OEE.Application.Features.Registros.RegistrarOEE.RegistrarOEECommand).Assembly,
        typeof(PlanTA.IA.Application.Features.Chat.EnviarMensaje.EnviarMensajeCommand).Assembly);

    cfg.AddOpenBehavior(typeof(LoggingBehavior<,>));
    cfg.AddOpenBehavior(typeof(ValidationBehavior<,>));
    cfg.AddOpenBehavior(typeof(AuditBehavior<,>));
    cfg.AddOpenBehavior(typeof(PerformanceBehavior<,>));
});

// -- Swagger --
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new()
    {
        Title = "PlanTA API",
        Version = "v1",
        Description = "ERP/MES ligero para PYMEs y pequenas fabricas"
    });
});

// -- CORS --
var allowedOrigins = builder.Configuration["Cors:AllowedOrigins"]
    ?? builder.Configuration["Frontend:Url"]
    ?? "http://localhost:4200";
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        if (builder.Environment.IsDevelopment())
        {
            policy.SetIsOriginAllowed(_ => true)
                .AllowAnyHeader()
                .AllowAnyMethod()
                .AllowCredentials();
        }
        else
        {
            policy.WithOrigins(allowedOrigins.Split(','))
                .AllowAnyHeader()
                .AllowAnyMethod()
                .AllowCredentials();
        }
    });
});

// -- Exception handling --
builder.Services.AddExceptionHandler<ValidationExceptionHandler>();
builder.Services.AddProblemDetails();

// -- Cross-cutting: Outbox --
builder.Services.AddDbContext<OutboxDbContext>(options =>
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        npgsql => npgsql.MigrationsHistoryTable("__EFMigrationsHistory", "shared")));

builder.Services.AddScoped<IOutboxStore, OutboxStore>();
builder.Services.AddSingleton<DomainEventInterceptor>();
builder.Services.AddHostedService<OutboxProcessor>();

// -- Cross-cutting: Audit --
builder.Services.AddDbContext<AuditDbContext>(options =>
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        npgsql => npgsql.MigrationsHistoryTable("__EFMigrationsHistory_Audit", "shared")));

builder.Services.AddScoped<IAuditStore, AuditStore>();

// -- Modulo Seguridad (Identity + JWT) --
builder.Services.AddSeguridadInfrastructure(builder.Configuration);

// -- Modulo Inventario --
builder.Services.AddInventarioInfrastructure(builder.Configuration);

// -- Modulo Produccion --
builder.Services.AddProduccionInfrastructure(builder.Configuration);

// -- Modulo Compras --
builder.Services.AddComprasInfrastructure(builder.Configuration);

// -- Modulo Ventas --
builder.Services.AddVentasInfrastructure(builder.Configuration);

// -- Modulo Calidad --
builder.Services.AddCalidadInfrastructure(builder.Configuration);

// -- Modulo Activos --
builder.Services.AddActivosInfrastructure(builder.Configuration);

// -- Modulo Mantenimiento --
builder.Services.AddMantenimientoInfrastructure(builder.Configuration);

// -- Modulo Incidencias --
builder.Services.AddIncidenciasInfrastructure(builder.Configuration);

// -- Modulo RRHH --
builder.Services.AddRRHHInfrastructure(builder.Configuration);

// -- Modulo Costes --
builder.Services.AddCostesInfrastructure(builder.Configuration);

// -- Modulo Importador --
builder.Services.AddImportadorInfrastructure(builder.Configuration);

// -- Modulo CRM --
builder.Services.AddCrmInfrastructure(builder.Configuration);

// -- Modulo Facturacion --
builder.Services.AddFacturacionInfrastructure(builder.Configuration);

// -- Modulo OEE --
builder.Services.AddOEEInfrastructure(builder.Configuration);

// -- Modulo IA --
builder.Services.AddIAInfrastructure(builder.Configuration);

// -- Authorization --
builder.Services.AddAuthorization();

var app = builder.Build();

// -- Ensure schemas exist & seed data --
// EnsureCreatedAsync solo crea tablas si la BD no tiene NINGUNA tabla.
// Con múltiples DbContexts, solo el primero crea sus tablas y el resto se salta.
// Usamos CreateTablesAsync por cada DbContext para forzar la creación.
async Task EnsureTablesAsync(DbContext db)
{
    try
    {
        var creator = db.GetService<IRelationalDatabaseCreator>();
        await creator.CreateTablesAsync();
    }
    catch (Npgsql.PostgresException ex) when (ex.SqlState == "42P07")
    {
        // 42P07 = "relation already exists" — tablas ya creadas, ignorar
    }
}

using (var scope = app.Services.CreateScope())
{
    // Asegurar que la BD existe (solo la primera vez)
    var outboxDb = scope.ServiceProvider.GetRequiredService<OutboxDbContext>();
    await outboxDb.Database.EnsureCreatedAsync();

    // Crear tablas de cada módulo (CreateTablesAsync funciona con múltiples DbContexts)
    var auditDb = scope.ServiceProvider.GetRequiredService<AuditDbContext>();
    await EnsureTablesAsync(auditDb);

    var segDb = scope.ServiceProvider.GetRequiredService<SeguridadDbContext>();
    await EnsureTablesAsync(segDb);

    await EnsureTablesAsync(scope.ServiceProvider.GetRequiredService<InventarioDbContext>());
    await EnsureTablesAsync(scope.ServiceProvider.GetRequiredService<ProduccionDbContext>());
    await EnsureTablesAsync(scope.ServiceProvider.GetRequiredService<ComprasDbContext>());
    await EnsureTablesAsync(scope.ServiceProvider.GetRequiredService<VentasDbContext>());
    await EnsureTablesAsync(scope.ServiceProvider.GetRequiredService<CalidadDbContext>());
    await EnsureTablesAsync(scope.ServiceProvider.GetRequiredService<ActivosDbContext>());
    await EnsureTablesAsync(scope.ServiceProvider.GetRequiredService<MantenimientoDbContext>());
    await EnsureTablesAsync(scope.ServiceProvider.GetRequiredService<IncidenciasDbContext>());
    await EnsureTablesAsync(scope.ServiceProvider.GetRequiredService<RRHHDbContext>());
    await EnsureTablesAsync(scope.ServiceProvider.GetRequiredService<CostesDbContext>());
    await EnsureTablesAsync(scope.ServiceProvider.GetRequiredService<ImportadorDbContext>());
    await EnsureTablesAsync(scope.ServiceProvider.GetRequiredService<CrmDbContext>());
    await EnsureTablesAsync(scope.ServiceProvider.GetRequiredService<FacturacionDbContext>());
    await EnsureTablesAsync(scope.ServiceProvider.GetRequiredService<OEEDbContext>());
    await EnsureTablesAsync(scope.ServiceProvider.GetRequiredService<IADbContext>());

    // Seed roles
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole<Guid>>>();
    string[] roles = ["Administrador", "GerentePlanta", "JefeAlmacen", "JefeProduccion", "Compras", "Ventas", "Calidad", "Operario"];
    foreach (var rol in roles)
    {
        if (!await roleManager.RoleExistsAsync(rol))
            await roleManager.CreateAsync(new IdentityRole<Guid> { Name = rol });
    }

    // Seed empresa
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
    if (!await userManager.Users.AnyAsync())
    {
        var empresa = Empresa.Crear("PlanTA Demo", "B12345678", "demo@planta-erp.com");
        segDb.Empresas.Add(empresa);
        await segDb.SaveChangesAsync();

        // Seed users
        var seedUsers = new[]
        {
            ("admin@planta-erp.com", "Admin2026!!", "Administrador", "Administrador"),
            ("gerente@planta-erp.com", "Gerente2026!!", "Gerente de Planta", "GerentePlanta"),
            ("calidad@planta-erp.com", "Calidad2026!!", "Jefe de Calidad", "Calidad"),
            ("operario@planta-erp.com", "Operario2026!!", "Operario Demo", "Operario"),
        };

        foreach (var (email, password, nombre, rol) in seedUsers)
        {
            var user = new ApplicationUser
            {
                UserName = email,
                Email = email,
                Nombre = nombre,
                EmpresaId = empresa.Id.Value
            };
            var res = await userManager.CreateAsync(user, password);
            if (res.Succeeded) await userManager.AddToRoleAsync(user, rol);
        }
    }
}

// -- Demo seed CLI mode --
if (args.Contains("--seed-demo"))
{
    var logger = app.Services.GetRequiredService<ILoggerFactory>().CreateLogger("DemoSeed");
    try
    {
        await PlanTA.API.DemoDataSeeder.RunAsync(app.Services, logger);
        return;
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "Demo seed FALLÓ");
        Environment.ExitCode = 1;
        return;
    }
}

// -- Middleware pipeline --
app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "PlanTA API v1"));
}

app.UseSerilogRequestLogging();
app.UseCors();
app.UseAuthentication();
app.UseAuthorization();

// -- Map all endpoint groups via reflection --
app.MapEndpointGroups();

// -- Debug endpoint (quitar tras diagnóstico) --
app.MapGet("/api/v1/debug/whoami", (HttpContext ctx, PlanTA.SharedKernel.ICurrentTenant tenant) =>
{
    return Results.Ok(new
    {
        isAuthenticated = ctx.User?.Identity?.IsAuthenticated ?? false,
        claims = ctx.User?.Claims.Select(c => new { c.Type, c.Value }).ToArray(),
        tenantEmpresaId = tenant.EmpresaId,
        tenantUserId = tenant.UserId,
        tenantRole = tenant.Role,
        buildMarker = "2026-04-14-tenant-fix"
    });
});

app.Run();
