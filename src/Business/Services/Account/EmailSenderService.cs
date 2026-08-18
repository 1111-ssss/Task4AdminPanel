using Business.Interfaces.Account;
using FluentEmail.Core;
using Microsoft.Extensions.Logging;
using Business.Constants;

namespace Business.Services.Account;

public class EmailSenderService : IEmailSenderService
{
    private readonly IFluentEmail _fluentEmail;
    private readonly ILogger<EmailSenderService> _logger;

    public EmailSenderService(
        IFluentEmail fluentEmail,
        ILogger<EmailSenderService> logger
    )
    {
        _fluentEmail = fluentEmail;
        _logger = logger;
    }

    public async Task SendConfirmationEmail(string email, string link, string token)
    {
        var confirmationLink = link + token;

        _logger.LogInformation($"Sending confirmation email to {email}");
        
        await _fluentEmail
            .To(email)
            .Subject(EmailSenderConstants.EMAIL_SUBJECT)
            .Body(
                string.Format(EmailSenderConstants.EMAIL_BODY, confirmationLink),
                isHtml: true
            )
            .SendAsync();

        _logger.LogInformation($"Confirmation email sent to {email}");
    }

    public string GenerateEmailConfirmationToken()
    {
        var token = $"{Guid.NewGuid().ToString()}-{Guid.NewGuid().ToString()}";

        return token;
    }

    public DateTime GetEmailConfirmationExpiration()
    {
        return DateTime.UtcNow.AddMinutes(EmailSenderConstants.EMAIL_CONFIRMATION_TOKEN_EXPIRATION_MINUTES);
    }
}