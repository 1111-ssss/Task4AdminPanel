namespace Data.Interfaces.Services;

public interface IDatabaseSeeder
{
    void SetDefaultPassword(string password);
    Task SeedDatabase(CancellationToken cancellationToken = default);
}