using Data.Enums;

namespace Business.Contracts.Account;

public record UserResponse(
    string Name,
    string Surname,
    string Email,
    DateTime RegistrationTime,
    DateTime LastLoginTime,
    UserStatus Status
);