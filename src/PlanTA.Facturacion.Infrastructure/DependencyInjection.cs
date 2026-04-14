using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PlanTA.Facturacion.Application.Interfaces;
using PlanTA.Facturacion.Infrastructure.Data;
using PlanTA.Facturacion.Infrastructure.Services;
using PlanTA.SharedKernel.Outbox;

namespace PlanTA.Facturacion.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddFacturacionInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContext<FacturacionDbContext>((sp, options) =>
        {
            options.UseNpgsql(
                configuration.GetConnectionString("DefaultConnection"),
                npgsql => npgsql.MigrationsHistoryTable("__EFMigrationsHistory", "facturacion"));

            var interceptor = sp.GetService<DomainEventInterceptor>();
            if (interceptor is not null)
                options.AddInterceptors(interceptor);
        });

        services.AddScoped<IFacturacionDbContext>(sp => sp.GetRequiredService<FacturacionDbContext>());
        services.AddScoped<IVerifactuService, VerifactuMockService>();

        return services;
    }
}
