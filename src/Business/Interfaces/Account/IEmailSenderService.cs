namespace Business.Interfaces.Account;

public interface IEmailSenderService
{
    Task SendConfirmationEmail(string email, string link, string token, CancellationToken cancellationToken);
    string GenerateEmailConfirmationToken();
    DateTime GetEmailConfirmationExpiration();
}