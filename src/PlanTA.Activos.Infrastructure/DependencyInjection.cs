using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PlanTA.Activos.Application.Interfaces;
using PlanTA.Activos.Infrastructure.Data;
using PlanTA.SharedKernel.Outbox;

namespace PlanTA.Activos.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddActivosInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContext<ActivosDbContext>((sp, options) =>
        {
            options.UseNpgsql(
                configuration.GetConnectionString("DefaultConnection"),
                npgsql => npgsql.MigrationsHistoryTable("__EFMigrationsHistory", "activos"));

            var interceptor = sp.GetService<DomainEventInterceptor>();
            if (interceptor is not null)
                options.AddInterceptors(interceptor);
        });

        services.AddScoped<IActivosDbContext>(sp => sp.GetRequiredService<ActivosDbContext>());

        return services;
    }
}
