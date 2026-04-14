using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PlanTA.Incidencias.Application.Interfaces;
using PlanTA.Incidencias.Infrastructure.Data;
using PlanTA.SharedKernel.Outbox;

namespace PlanTA.Incidencias.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddIncidenciasInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContext<IncidenciasDbContext>((sp, options) =>
        {
            options.UseNpgsql(
                configuration.GetConnectionString("DefaultConnection"),
                npgsql => npgsql.MigrationsHistoryTable("__EFMigrationsHistory", "incidencias"));

            var interceptor = sp.GetService<DomainEventInterceptor>();
            if (interceptor is not null)
                options.AddInterceptors(interceptor);
        });

        services.AddScoped<IIncidenciasDbContext>(sp => sp.GetRequiredService<IncidenciasDbContext>());
        return services;
    }
}
