using Business.Common.Result;
using Business.Common.Errors;
using Business.Contracts.Account;
using Business.Interfaces.Account;
using Data.Enums;
using Data.Interfaces.Repositories;
using Data.Interfaces.Services;
using FluentValidation;
using Microsoft.Extensions.Logging;

namespace Business.Services.Account;

public class AccountLoginService : ServiceValidation, IAccountLoginService
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IEmailSenderService _emailSenderService;
    private readonly IValidator<LoginUserRequest> _loginUserValidator;
    private readonly ILogger<AccountLoginService> _logger;

    public AccountLoginService(
        IUserRepository userRepository,
        IPasswordHasher passwordHasher,
        IEmailSenderService emailSenderService,
        IValidator<LoginUserRequest> loginUserValidator,
        ILogger<AccountLoginService> logger
    )
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _emailSenderService = emailSenderService;
        _loginUserValidator = loginUserValidator;
        _logger = logger;
    }

    public async Task<Result<UserResponse>> Login(LoginUserRequest request, Func<string> getLink)
    {
        var user = await _userRepository.GetByEmail(request.Email);
        if (user is null)
        {
            return Result.Failure(Errors.InvalidCredentials);
        }

        if (user.Status == UserStatus.Unverified && (user.EmailConfirmationExpiration is null || user.EmailConfirmationExpiration < DateTime.UtcNow))
        {
            await _emailSenderService.SendConfirmationEmail(user.Email, getLink());
        }

        if (user.Status == UserStatus.Blocked)
        {
            return Result.Failure(Errors.UserBlocked);
        }

        var validationResult = await Validate(_loginUserValidator, request);
        if (!validationResult.IsSuccess)
        {
            return validationResult;
        }

        if (!_passwordHasher.VerifyPassword(request.Password, user.PasswordHash))
        {
            return Result.Failure(Errors.InvalidCredentials);
        }

        try
        {
            user.LastLoginTime = DateTime.UtcNow;
            _userRepository.Update(user);

            await _userRepository.SaveChanges();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while updating user last login time");

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