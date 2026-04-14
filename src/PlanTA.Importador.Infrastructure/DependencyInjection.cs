using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PlanTA.Importador.Application.Interfaces;
using PlanTA.Importador.Infrastructure.Data;
using PlanTA.SharedKernel.Outbox;

namespace PlanTA.Importador.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddImportadorInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContext<ImportadorDbContext>((sp, options) =>
        {
            options.UseNpgsql(
                configuration.GetConnectionString("DefaultConnection"),
                npgsql => npgsql.MigrationsHistoryTable("__EFMigrationsHistory", "importacion"));

            var interceptor = sp.GetService<DomainEventInterceptor>();
            if (interceptor is not null)
                options.AddInterceptors(interceptor);
        });

        services.AddScoped<IImportadorDbContext>(sp => sp.GetRequiredService<ImportadorDbContext>());

        return services;
    }
}
