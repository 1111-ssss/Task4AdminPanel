namespace Business.Contracts.Account;

public record RegisterUserRequest(
    string Name,
    string Surname,
    string Email,
    string Password
);