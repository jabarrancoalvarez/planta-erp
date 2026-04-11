using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PlanTA.Compras.Application.Interfaces;
using PlanTA.Compras.Infrastructure.Data;
using PlanTA.SharedKernel.Outbox;

namespace PlanTA.Compras.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddComprasInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContext<ComprasDbContext>((sp, options) =>
        {
            options.UseNpgsql(
                configuration.GetConnectionString("DefaultConnection"),
                npgsql => npgsql.MigrationsHistoryTable("__EFMigrationsHistory", "compras"));

            var interceptor = sp.GetService<DomainEventInterceptor>();
            if (interceptor is not null)
                options.AddInterceptors(interceptor);
        });

        services.AddScoped<IComprasDbContext>(sp => sp.GetRequiredService<ComprasDbContext>());

        return services;
    }
}
