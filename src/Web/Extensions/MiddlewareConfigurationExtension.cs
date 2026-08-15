using Data.Database;
using Microsoft.EntityFrameworkCore;

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

        app.UseHttpsRedirection();
        app.UseRouting();
        app.UseAuthorization();
        app.MapStaticAssets();
        app.MapRazorPages()
            .WithStaticAssets();

        using (var scope = app.Services.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            dbContext.Database.Migrate();
        }

        return app;
    }
}