using System.Net;
using System.Net.Mail;
using Business;
using Business.Interfaces.Account;
using Business.Interfaces.Admin;
using Business.Services.Account;
using Business.Services.Admin;
using Data.Interfaces.Services;
using Data.Services;
using FluentEmail.MailKitSmtp;
using FluentValidation;
using Web.Interfaces;
using Web.Services;

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
        services.AddScoped<IListUsersService, ListUsersService>();
        services.AddScoped<IDeleteUserService, DeleteUserService>();
        services.AddScoped<IBlockUserService, BlockUserService>();

        // Data Services
        services.AddScoped<IPasswordHasher, BCryptPasswordHasher>();

        // Web Services
        services.AddScoped<IAuthCookieService, AuthCookieService>();

        // Validators
        services.AddValidatorsFromAssembly(typeof(AssemblyMarker).Assembly);

        services
            .AddFluentEmail(configuration["Email:From"])
            .AddMailKitSender(new SmtpClientOptions
            {
                Server = configuration["Email:SmtpServer"],
                Port = int.Parse(configuration["Email:Port"]!),
                UseSsl = true,
                RequiresAuthentication = true,
                User = configuration["Email:From"],
                Password = configuration["Email:Password"]
            });

        return services;
    }
}