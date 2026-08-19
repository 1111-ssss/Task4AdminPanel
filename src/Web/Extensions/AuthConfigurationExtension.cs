using Microsoft.AspNetCore.Authentication.Cookies;

namespace Web.Extensions;

public static class AuthConfigurationExtension
{
    public static IServiceCollection AddAuthConfiguration(this IServiceCollection services)
    {
        services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
            .AddCookie(options =>
            {
                options.LoginPath = "/Account/Login";
                options.AccessDeniedPath = "/Account/Login";
                options.ExpireTimeSpan = TimeSpan.FromDays(7);
                options.SlidingExpiration = true;

                options.Cookie.HttpOnly = true;
                options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
            });

        services.AddAuthorization();

        services.AddHttpContextAccessor();

        return services;
    }
}