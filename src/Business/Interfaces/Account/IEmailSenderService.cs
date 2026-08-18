namespace Business.Interfaces.Account;

public interface IEmailSenderService
{
    Task SendConfirmationEmail(string email, string link, string token);
    string GenerateEmailConfirmationToken();
    DateTime GetEmailConfirmationExpiration();
}