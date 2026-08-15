using Business;
using Business.Interfaces.Account;
using Business.Services.Account;
using Data.Interfaces.Services;
using Data.Services;
using FluentValidation;

namespace Web.Extensions;

public static class ServiceConfigurationExtension
{
    public static IServiceCollection AddServiceConfiguration(this IServiceCollection services)
    {
        services.AddLogging();

        services.AddRazorPages();

        // Services
        services.AddScoped<IAccountRegisterService, AccountRegisterService>();
        services.AddScoped<IPasswordHasher, BCryptPasswordHasher>();

        // Validators
        services.AddValidatorsFromAssembly(typeof(AssemblyMarker).Assembly);

        return services;
    }
}