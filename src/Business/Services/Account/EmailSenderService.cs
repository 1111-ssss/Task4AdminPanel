using Ardalis.Result;
using Business.Interfaces.Account;
using Data.Enums;
using Data.Interfaces.Repositories;
using FluentEmail.Core;
using Microsoft.Extensions.Logging;

namespace Business.Services.Account;

public class EmailSenderService : IEmailSenderService
{
    private const int EMAIL_CONFIRMATION_TOKEN_EXPIRATION_MINUTES = 30;

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

    public async Task<Result> SendConfirmationEmail(string email)
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

        // TODO: Move body to a separate file, remove hard-coded values
        var body = $@"
            <h2>Email confirmation</h2>
            <p>To confirm your email, please click the link below:</p>
            <a href='{generatedToken}'>Confirm email</a>
            <p>If you did not request this email, please ignore this message.</p>
        ";

        await _fluentEmail
            .To(email)
            .Subject("New login detected")
            .Body(body, isHtml: true)
            .SendAsync();

        try
        {
            user.EmailConfirmationToken = generatedToken;
            user.EmailConfirmationExpiration = DateTime.UtcNow.AddMinutes(EMAIL_CONFIRMATION_TOKEN_EXPIRATION_MINUTES);

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