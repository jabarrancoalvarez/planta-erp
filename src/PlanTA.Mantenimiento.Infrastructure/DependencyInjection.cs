using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PlanTA.Mantenimiento.Application.Interfaces;
using PlanTA.Mantenimiento.Infrastructure.Data;
using PlanTA.SharedKernel.Outbox;

namespace PlanTA.Mantenimiento.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddMantenimientoInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContext<MantenimientoDbContext>((sp, options) =>
        {
            options.UseNpgsql(
                configuration.GetConnectionString("DefaultConnection"),
                npgsql => npgsql.MigrationsHistoryTable("__EFMigrationsHistory", "mantenimiento"));

            var interceptor = sp.GetService<DomainEventInterceptor>();
            if (interceptor is not null)
                options.AddInterceptors(interceptor);
        });

        services.AddScoped<IMantenimientoDbContext>(sp => sp.GetRequiredService<MantenimientoDbContext>());
        return services;
    }
}
