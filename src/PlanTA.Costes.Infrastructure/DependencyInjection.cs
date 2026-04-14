using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PlanTA.Costes.Application.Interfaces;
using PlanTA.Costes.Infrastructure.Data;
using PlanTA.SharedKernel.Outbox;

namespace PlanTA.Costes.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddCostesInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContext<CostesDbContext>((sp, options) =>
        {
            options.UseNpgsql(
                configuration.GetConnectionString("DefaultConnection"),
                npgsql => npgsql.MigrationsHistoryTable("__EFMigrationsHistory", "costes"));

            var interceptor = sp.GetService<DomainEventInterceptor>();
            if (interceptor is not null)
                options.AddInterceptors(interceptor);
        });

        services.AddScoped<ICostesDbContext>(sp => sp.GetRequiredService<CostesDbContext>());

        return services;
    }
}
