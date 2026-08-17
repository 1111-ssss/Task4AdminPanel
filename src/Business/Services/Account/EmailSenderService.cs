using Ardalis.Result;
using Business.Interfaces.Account;
using Data.Common.Enums;
using Data.Interfaces.Repositories;
using FluentEmail.Core;
using Microsoft.Extensions.Logging;
using Business.Constants;

namespace Business.Services.Account;

public class EmailSenderService : IEmailSenderService
{
    private readonly IUserRepository _userRepository;
    private readonly IFluentEmail _fluentEmail;
    private readonly ILogger<EmailSenderService> _logger;

    public EmailSenderService(
        IUserRepository userRepository,
        IFluentEmail fluentEmail,
        ILogger<EmailSenderService> logger
    )
    {
        _userRepository = userRepository;
        _fluentEmail = fluentEmail;
        _logger = logger;
    }

    public async Task<Result> SendConfirmationEmail(string email, string link)
    {
        var user = await _userRepository.GetByEmail(email);
        if (user is null)
        {
            return Result.NotFound("User not found");
        }

        if (user.Status != UserStatus.Unverified)
        {
            return Result.Conflict("User is already verified or blocked");
        }

        if (user.EmailConfirmationToken is not null && user.EmailConfirmationExpiration > DateTime.UtcNow)
        {
            return Result.Conflict("Email confirmation token already sent and is still valid");
        }

        var generatedToken = GenerateEmailConfirmationToken();
        var confirmationLink = link + generatedToken;

        _logger.LogInformation($"Sending confirmation email to {email}");
        
        await _fluentEmail
            .To(email)
            .Subject("New login detected")
            .Body(
                string.Format(EmailSenderConstants.EMAIL_BODY, confirmationLink),
                isHtml: true
            )
            .SendAsync();

        _logger.LogInformation($"Confirmation email sent to {email}");

        try
        {
            user.EmailConfirmationToken = generatedToken;
            user.EmailConfirmationExpiration = DateTime.UtcNow.AddMinutes(
                EmailSenderConstants.EMAIL_CONFIRMATION_TOKEN_EXPIRATION_MINUTES
            );

            _userRepository.Update(user);

            await _userRepository.SaveChanges();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while sending confirmation email");

            return Result.Error("Error while sending confirmation email");
        }

        return Result.Success();
    }

    private string GenerateEmailConfirmationToken()
    {
        var token = $"{Guid.NewGuid().ToString()}-{Guid.NewGuid().ToString()}";

        return token;
    }
}