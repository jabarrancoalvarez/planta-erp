using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PlanTA.Ventas.Application.Interfaces;
using PlanTA.Ventas.Infrastructure.Data;
using PlanTA.SharedKernel.Outbox;

namespace PlanTA.Ventas.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddVentasInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContext<VentasDbContext>((sp, options) =>
        {
            options.UseNpgsql(
                configuration.GetConnectionString("DefaultConnection"),
                npgsql => npgsql.MigrationsHistoryTable("__EFMigrationsHistory", "ventas"));

            var interceptor = sp.GetService<DomainEventInterceptor>();
            if (interceptor is not null)
                options.AddInterceptors(interceptor);
        });

        services.AddScoped<IVentasDbContext>(sp => sp.GetRequiredService<VentasDbContext>());

        return services;
    }
}
