using Web.Middleware;

namespace Web.Extensions;

public static class MiddlewareConfigurationExtension
{
    public static WebApplication AddMiddlewareConfiguration(this WebApplication app)
    {
        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Error");
            app.UseHsts();
        }

        app.UseMiddleware<GlobalExceptionHandlerMiddleware>();
        app.UseHttpsRedirection();
        app.UseRouting();
        app.UseAuthentication();
        app.UseMiddleware<UserStatusCheckMiddleware>();
        app.UseAuthorization();
        app.MapStaticAssets();
        app.MapRazorPages()
            .WithStaticAssets();

        return app;
    }
}