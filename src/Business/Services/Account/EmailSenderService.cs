using Business.Interfaces.Account;
using Microsoft.Extensions.Logging;
using Business.Constants;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using Microsoft.Extensions.Configuration;

namespace Business.Services.Account;

public class EmailSenderService : IEmailSenderService
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<EmailSenderService> _logger;

    public EmailSenderService(
        IConfiguration configuration,
        ILogger<EmailSenderService> logger
    )
    {
        _configuration = configuration;
        _logger = logger;

        ArgumentException.ThrowIfNullOrEmpty(_configuration["Email:From"]);
        ArgumentException.ThrowIfNullOrEmpty(_configuration["Email:SmtpServer"]);
        ArgumentException.ThrowIfNullOrEmpty(_configuration["Email:Password"]);
    }

    public async Task SendConfirmationEmail(string email, string link, string token, CancellationToken cancellationToken)
    {
        var email_from = _configuration["Email:From"]!;
        var email_smtp_server = _configuration["Email:SmtpServer"]!;
        var email_password = _configuration["Email:Password"]!;

        var port = int.Parse(_configuration["Email:Port"] ?? "587");
        var socketOption = port == 465 ? SecureSocketOptions.SslOnConnect : SecureSocketOptions.StartTls;

        var confirmationLink = link + token;
        _logger.LogInformation("Sending confirmation email to {Email}", email);

        var message = new MimeMessage();
        message.From.Add(MailboxAddress.Parse(email_from));
        message.To.Add(MailboxAddress.Parse(email));
        message.Subject = EmailSenderConstants.EMAIL_SUBJECT;
        message.Body = new TextPart("html")
        {
            Text = string.Format(EmailSenderConstants.EMAIL_BODY, confirmationLink)
        };

        using var client = new SmtpClient();

        await client.ConnectAsync(
            email_smtp_server,
            port,
            socketOption,
            cancellationToken
        );

        await client.AuthenticateAsync(
            email_from,
            email_password,
            cancellationToken
        );

        var result = await client.SendAsync(message, cancellationToken);
        _logger.LogInformation($"Send Email Result: {result}");

        await client.DisconnectAsync(true, cancellationToken);
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