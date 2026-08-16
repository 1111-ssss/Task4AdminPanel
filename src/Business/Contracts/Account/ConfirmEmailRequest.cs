namespace Business.Contracts.Account;

public record ConfirmEmailRequest(
    string Token
);