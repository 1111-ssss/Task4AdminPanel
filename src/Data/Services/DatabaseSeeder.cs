using Bogus;
using Data.Interfaces.Services;
using Data.Entities;
using Data.Enums;
using Data.Interfaces.Repositories;
using Microsoft.Extensions.Logging;

namespace Data.Services;

public class DatabaseSeeder : IDatabaseSeeder
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly ILogger<DatabaseSeeder> _logger;
    private string _defaultPassword = string.Empty;

    public DatabaseSeeder(
        IUserRepository userRepository,
        IPasswordHasher passwordHasher,
        ILogger<DatabaseSeeder> logger
    )
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _logger = logger;
    }

    public void SetDefaultPassword(string password)
    {
        var hashedPassword = _passwordHasher.HashPassword(password);
        _defaultPassword = hashedPassword;
        _logger.LogInformation($"Default password for seeding set to {hashedPassword}");
    }

    public async Task SeedDatabase(CancellationToken cancellationToken = default)
    {
        if (await _userRepository.AnyAsync(cancellationToken))
        {
            _logger.LogInformation("Database already seeded");
            return;
        }

        ArgumentException.ThrowIfNullOrEmpty(_defaultPassword);

        var statuses = new[]
        {
            UserStatus.Active,
            UserStatus.Blocked,
            UserStatus.Unverified
        };

        var faker = new Faker<ApplicationUser>("en")
            .RuleFor(u => u.Email, f => f.Internet.Email().ToLower())
            .RuleFor(u => u.Name, f => f.Name.FirstName())
            .RuleFor(u => u.Surname, f => f.Name.LastName())
            .RuleFor(u => u.RegistrationTime, f => f.Date.Past(2).ToUniversalTime())
            .RuleFor(u => u.LastLoginTime, (f, u) =>
                f.Date.Between(u.RegistrationTime, DateTime.UtcNow).ToUniversalTime())
            .RuleFor(u => u.Status, f => f.PickRandom(statuses))
            .RuleFor(u => u.PasswordHash, _ => _defaultPassword);

        var users = faker.Generate(30);

        try
        {
            _userRepository.AddRange(users);
            await _userRepository.SaveChanges(cancellationToken);

            _logger.LogInformation($"Seeded {users.Count} users");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while seeding users");
            throw;
        }
    }
}