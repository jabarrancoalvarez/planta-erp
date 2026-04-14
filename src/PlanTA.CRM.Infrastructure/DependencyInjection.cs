using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PlanTA.CRM.Application.Interfaces;
using PlanTA.CRM.Infrastructure.Data;
using PlanTA.SharedKernel.Outbox;

namespace PlanTA.CRM.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddCrmInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContext<CrmDbContext>((sp, options) =>
        {
            options.UseNpgsql(
                configuration.GetConnectionString("DefaultConnection"),
                npgsql => npgsql.MigrationsHistoryTable("__EFMigrationsHistory", "crm"));

            var interceptor = sp.GetService<DomainEventInterceptor>();
            if (interceptor is not null)
                options.AddInterceptors(interceptor);
        });

        services.AddScoped<ICrmDbContext>(sp => sp.GetRequiredService<CrmDbContext>());

        return services;
    }
}
