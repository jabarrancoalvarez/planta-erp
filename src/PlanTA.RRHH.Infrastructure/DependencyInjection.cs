using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PlanTA.RRHH.Application.Interfaces;
using PlanTA.RRHH.Infrastructure.Data;
using PlanTA.SharedKernel.Outbox;

namespace PlanTA.RRHH.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddRRHHInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContext<RRHHDbContext>((sp, options) =>
        {
            options.UseNpgsql(
                configuration.GetConnectionString("DefaultConnection"),
                npgsql => npgsql.MigrationsHistoryTable("__EFMigrationsHistory", "rrhh"));

            var interceptor = sp.GetService<DomainEventInterceptor>();
            if (interceptor is not null)
                options.AddInterceptors(interceptor);
        });

        services.AddScoped<IRRHHDbContext>(sp => sp.GetRequiredService<RRHHDbContext>());

        return services;
    }
}
