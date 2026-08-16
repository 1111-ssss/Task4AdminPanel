using Data.Database;
using Data.Interfaces.Repositories;
using Data.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Web.Extensions;

public static class DatabaseConfigurationExtension
{
    public static IServiceCollection AddDatabaseConfiguration(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<AppDbContext>(options =>
            options.UseNpgsql(
                configuration.GetConnectionString("DefaultConnection")
            )
        );

        // Repositories
        services.AddScoped<IUserRepository, UserRepository>();

        return services;
    }
}