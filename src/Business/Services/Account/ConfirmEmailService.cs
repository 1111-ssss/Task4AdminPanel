using Business.Common.Result;
using Business.Common.Errors;
using Business.Contracts.Account;
using Business.Interfaces.Account;
using Data.Enums;
using Data.Interfaces.Repositories;
using FluentValidation;
using Microsoft.Extensions.Logging;

namespace Business.Services.Account;

public class ConfirmEmailService : ServiceValidation, IConfirmEmailService
{
    private readonly IUserRepository _userRepository;
    private readonly IValidator<ConfirmEmailRequest> _confirmEmailValidator;
    private readonly ILogger<ConfirmEmailService> _logger;

    public ConfirmEmailService(
        IUserRepository userRepository,
        IValidator<ConfirmEmailRequest> confirmEmailValidator,
        ILogger<ConfirmEmailService> logger
    )
    {
        _userRepository = userRepository;
        _confirmEmailValidator = confirmEmailValidator;
        _logger = logger;
    }

    public async Task<Result<UserResponse>> ConfirmEmail(ConfirmEmailRequest request)
    {
        var validationResult = await Validate(_confirmEmailValidator, request);
        if (!validationResult.IsSuccess)
        {
            return validationResult;
        }

        var user = await _userRepository.GetByEmailConfirmationToken(request.Token);
        if (user is null)
        {
            return Result.Failure(Errors.InvalidEmailToken);
        }

        if (user.Status != UserStatus.Unverified)
        {
            return Result.Failure(Errors.UserIsVerifiedOrBlocked);
        }

        if (user.EmailConfirmationExpiration is null || user.EmailConfirmationExpiration < DateTime.UtcNow)
        {
            return Result.Failure(Errors.TokenExpired);
        }

        try {
            user.Status = UserStatus.Active;
            user.EmailConfirmationToken = null;
            user.EmailConfirmationExpiration = null;

            _userRepository.Update(user);

            await _userRepository.SaveChanges();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while confirming email");

            return Result.Failure(Errors.DatabaseError);
        }

        return Result<UserResponse>.Success(
            new UserResponse(
                Name: user.Name,
                Surname: user.Surname,
                Email: user.Email,
                RegistrationTime: user.RegistrationTime,
                LastLoginTime: user.LastLoginTime,
                Status: user.Status
            )
        );
    }
}