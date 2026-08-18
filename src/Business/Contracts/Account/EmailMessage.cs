namespace Business.Contracts.Account;

public record EmailMessage(
    string ToEmail,
    string ConfirmationLink,
    string Token
);