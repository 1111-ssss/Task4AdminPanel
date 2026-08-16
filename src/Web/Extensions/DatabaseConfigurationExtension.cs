using Data.Database;
using Data.Interfaces.Repositories;
using Data.Repositories;

namespace Web.Extensions;

public static class DatabaseConfigurationExtension
{
    public static IServiceCollection AddDatabaseConfiguration(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDatabase(configuration);

        // Repositories
        services.AddScoped<IUserRepository, UserRepository>();

        return services;
    }
}