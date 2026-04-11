using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PlanTA.Calidad.Application.Interfaces;
using PlanTA.Calidad.Infrastructure.Data;
using PlanTA.SharedKernel.Outbox;

namespace PlanTA.Calidad.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddCalidadInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContext<CalidadDbContext>((sp, options) =>
        {
            options.UseNpgsql(
                configuration.GetConnectionString("DefaultConnection"),
                npgsql => npgsql.MigrationsHistoryTable("__EFMigrationsHistory", "calidad"));

            var interceptor = sp.GetService<DomainEventInterceptor>();
            if (interceptor is not null)
                options.AddInterceptors(interceptor);
        });

        services.AddScoped<ICalidadDbContext>(sp => sp.GetRequiredService<CalidadDbContext>());

        return services;
    }
}
