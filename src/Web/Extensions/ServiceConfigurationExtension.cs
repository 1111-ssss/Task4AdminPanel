using Business;
using Business.Interfaces.Account;
using Business.Services.Account;
using Data.Interfaces.Services;
using Data.Services;
using FluentValidation;

namespace Web.Extensions;

public static class ServiceConfigurationExtension
{
    public static IServiceCollection AddServiceConfiguration(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddLogging();

        services.AddRazorPages();

        // Business Services
        services.AddScoped<IAccountRegisterService, AccountRegisterService>();
        services.AddScoped<IAccountLoginService, AccountLoginService>();
        services.AddScoped<IConfirmEmailService, ConfirmEmailService>();
        services.AddScoped<IEmailSenderService, EmailSenderService>();

        // Data Services
        services.AddScoped<IPasswordHasher, BCryptPasswordHasher>();

        // Validators
        services.AddValidatorsFromAssembly(typeof(AssemblyMarker).Assembly);

        services
            .AddFluentEmail(configuration["Email:From"])
            .AddSmtpSender(
                configuration["Email:SmtpServer"],
                int.Parse(configuration["Email:Port"] ?? "587"),
                configuration["Email:From"],
                configuration["Email:Password"]
            );

        return services;
    }
}