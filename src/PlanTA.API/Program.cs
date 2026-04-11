using Microsoft.EntityFrameworkCore;
using PlanTA.API.Endpoints;
using PlanTA.API.Infrastructure;
using PlanTA.API.Middleware;
using PlanTA.Calidad.Infrastructure;
using PlanTA.Compras.Infrastructure;
using PlanTA.Inventario.Infrastructure;
using PlanTA.Produccion.Infrastructure;
using PlanTA.Seguridad.Infrastructure;
using PlanTA.SharedKernel.Audit;
using PlanTA.SharedKernel.Behaviors;
using PlanTA.SharedKernel.Outbox;
using PlanTA.Ventas.Infrastructure;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// -- Serilog --
builder.Host.UseSerilog((context, config) =>
    config.ReadFrom.Configuration(context.Configuration));

// -- MediatR + Behaviors --
builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssemblies(
        AppDomain.CurrentDomain.GetAssemblies()
            .Where(a => a.FullName?.Contains("PlanTA") == true)
            .ToArray());

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

// -- Authorization --
builder.Services.AddAuthorization();

var app = builder.Build();

// -- Ensure shared schema exists --
using (var scope = app.Services.CreateScope())
{
    var outboxDb = scope.ServiceProvider.GetRequiredService<OutboxDbContext>();
    await outboxDb.Database.EnsureCreatedAsync();

    var auditDb = scope.ServiceProvider.GetRequiredService<AuditDbContext>();
    await auditDb.Database.EnsureCreatedAsync();
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

app.Run();
