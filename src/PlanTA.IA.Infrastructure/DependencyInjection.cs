using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PlanTA.IA.Application.Interfaces;
using PlanTA.IA.Infrastructure.Data;
using PlanTA.IA.Infrastructure.Services;
using PlanTA.SharedKernel.Outbox;

namespace PlanTA.IA.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddIAInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContext<IADbContext>((sp, options) =>
        {
            options.UseNpgsql(
                configuration.GetConnectionString("DefaultConnection"),
                npgsql => npgsql.MigrationsHistoryTable("__EFMigrationsHistory", "ia"));

            var interceptor = sp.GetService<DomainEventInterceptor>();
            if (interceptor is not null)
                options.AddInterceptors(interceptor);
        });

        services.AddScoped<IIADbContext>(sp => sp.GetRequiredService<IADbContext>());
        services.AddHttpClient<IClaudeService, ClaudeService>();

        return services;
    }
}
