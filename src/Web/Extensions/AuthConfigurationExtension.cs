namespace Web.Extensions;

public static class AuthConfigurationExtension
{
    public static IServiceCollection AddAuthConfiguration(this IServiceCollection services)
    {
        services.AddAuthorization();

        return services;
    }
}