namespace Business.Contracts.Account;

public record LoginUserRequest(
    string Email,
    string Password,
    bool RememberMe
);