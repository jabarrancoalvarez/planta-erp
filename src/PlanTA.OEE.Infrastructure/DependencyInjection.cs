using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PlanTA.OEE.Application.Interfaces;
using PlanTA.OEE.Infrastructure.Data;
using PlanTA.SharedKernel.Outbox;

namespace PlanTA.OEE.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddOEEInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContext<OEEDbContext>((sp, options) =>
        {
            options.UseNpgsql(
                configuration.GetConnectionString("DefaultConnection"),
                npgsql => npgsql.MigrationsHistoryTable("__EFMigrationsHistory", "oee"));

            var interceptor = sp.GetService<DomainEventInterceptor>();
            if (interceptor is not null)
                options.AddInterceptors(interceptor);
        });

        services.AddScoped<IOEEDbContext>(sp => sp.GetRequiredService<OEEDbContext>());

        return services;
    }
}
