using Data.Database;
using Data.Interfaces.Services;
using Microsoft.EntityFrameworkCore;

namespace Web.Extensions;

public static class ApplicationConfigurationExtension
{
    public static async Task<WebApplication> AddApplicationConfiguration(this WebApplication app)
    {
        using (var scope = app.Services.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            await dbContext.Database.MigrateAsync();

            if (app.Environment.IsDevelopment())
            {
                var defaultPassword = app.Configuration["DatabaseSeeder:DefaultPassword"];
                ArgumentException.ThrowIfNullOrEmpty(defaultPassword);

                var dbSeeder = scope.ServiceProvider.GetRequiredService<IDatabaseSeeder>();
                dbSeeder.SetDefaultPassword(defaultPassword);
                await dbSeeder.SeedDatabase();
            }
        }

        return app;
    }
}