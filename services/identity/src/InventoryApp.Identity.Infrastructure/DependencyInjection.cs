using InventoryApp.Identity.Application.Interfaces;
using InventoryApp.Identity.Infrastructure.Authentication;
using InventoryApp.Identity.Infrastructure.Persistence;
using InventoryApp.Identity.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace InventoryApp.Identity.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddIdentityInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var provider = configuration.GetSection("PersistenceSettings")["Provider"] ?? "Postgres";

        services.AddDbContext<IdentityDbContext>(options =>
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
                options.UseInMemoryDatabase("IdentityDb");
            }
        });

        services.AddScoped<IIdentityDbContext>(sp => sp.GetRequiredService<IdentityDbContext>());
        services.AddSingleton<IJwtTokenGenerator, JwtTokenGenerator>();
        services.AddScoped<IPasswordHasher, PasswordHasher>();

        return services;
    }
}
