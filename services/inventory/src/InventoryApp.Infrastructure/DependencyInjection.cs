using InventoryApp.Application.Interfaces;
using InventoryApp.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace InventoryApp.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var provider = configuration.GetSection("PersistenceSettings")["Provider"] ?? "Postgres";

        services.AddDbContext<ApplicationDbContext>(options =>
        {
            if (provider == "Postgres")
            {
                var postgresConnectionString = configuration.GetConnectionString("PostgresConnection")
                    ?? "Host=localhost;Port=5432;Database=inventory;Username=inventory;Password=inventory123";
                options.UseNpgsql(postgresConnectionString, npgsql =>
                    npgsql.MigrationsHistoryTable("__EFMigrationsHistory", "public"));
            }
            else
            {
                options.UseInMemoryDatabase("InventoryDb");
            }
        });

        services.AddScoped<IApplicationDbContext>(sp =>
            sp.GetRequiredService<ApplicationDbContext>());

        return services;
    }
}
