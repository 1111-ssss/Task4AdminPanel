using Data.Interfaces;
using Data.Common.Enums;

namespace Data.Entities;

public class ApplicationUser : IEntity
{
    public int Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Surname { get; set; } = string.Empty;
    public DateTime RegistrationTime { get; set; }
    public DateTime LastLoginTime { get; set; }
    public UserStatus Status { get; set; } = UserStatus.Unverified;
    public string PasswordHash { get; set; } = string.Empty;
    public string? EmailConfirmationToken { get; set; }
    public DateTime? EmailConfirmationExpiration { get; set; }
}