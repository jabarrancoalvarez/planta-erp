using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PlanTA.Inventario.Application.Interfaces;
using PlanTA.Inventario.Infrastructure.Data;
using PlanTA.SharedKernel.Outbox;

namespace PlanTA.Inventario.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInventarioInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContext<InventarioDbContext>((sp, options) =>
        {
            options.UseNpgsql(
                configuration.GetConnectionString("DefaultConnection"),
                npgsql => npgsql.MigrationsHistoryTable("__EFMigrationsHistory", "inventario"));

            var interceptor = sp.GetService<DomainEventInterceptor>();
            if (interceptor is not null)
                options.AddInterceptors(interceptor);
        });

        services.AddScoped<IInventarioDbContext>(sp => sp.GetRequiredService<InventarioDbContext>());

        return services;
    }
}
