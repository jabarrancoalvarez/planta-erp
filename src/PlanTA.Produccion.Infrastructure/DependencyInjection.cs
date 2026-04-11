using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PlanTA.Produccion.Application.Interfaces;
using PlanTA.Produccion.Infrastructure.Data;
using PlanTA.SharedKernel.Outbox;

namespace PlanTA.Produccion.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddProduccionInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContext<ProduccionDbContext>((sp, options) =>
        {
            options.UseNpgsql(
                configuration.GetConnectionString("DefaultConnection"),
                npgsql => npgsql.MigrationsHistoryTable("__EFMigrationsHistory", "produccion"));

            var interceptor = sp.GetService<DomainEventInterceptor>();
            if (interceptor is not null)
                options.AddInterceptors(interceptor);
        });

        services.AddScoped<IProduccionDbContext>(sp => sp.GetRequiredService<ProduccionDbContext>());

        return services;
    }
}
