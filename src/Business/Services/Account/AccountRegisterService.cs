using Data.Interfaces.Repositories;
using Business.Contracts.Account;
using Business.Interfaces.Account;
using Data.Interfaces.Services;
using Microsoft.Extensions.Logging;
using Business.Common.Result;
using Business.Common.Errors;
using FluentValidation;
using Data.Entities;
using Data.Enums;

namespace Business.Services.Account;

public class AccountRegisterService : ServiceValidation, IAccountRegisterService
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IValidator<RegisterUserRequest> _registerUserValidator;
    private readonly IEmailSenderService _emailSenderService;
    private readonly ILogger<AccountRegisterService> _logger;

    public AccountRegisterService(
        IUserRepository userRepository,
        IPasswordHasher passwordHasher,
        IValidator<RegisterUserRequest> registerUserValidator,
        IEmailSenderService emailSenderService,
        ILogger<AccountRegisterService> logger
    )
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _registerUserValidator = registerUserValidator;
        _emailSenderService = emailSenderService;
        _logger = logger;
    }
    
    public async Task<Result<UserResponse>> Register(RegisterUserRequest request, Func<string> getLink, CancellationToken cancellationToken)
    {
        var validationResult = await Validate(_registerUserValidator, request, cancellationToken);
        if (!validationResult.IsSuccess)
        {
            return validationResult;
        }

        var user = await _userRepository.GetByEmail(request.Email, cancellationToken);
        if (user is not null)
        {
            return Result.Failure(Errors.EmailAlreadyExists);
        }

        var passwordHash = _passwordHasher.HashPassword(request.Password);

        var token = _emailSenderService.GenerateEmailConfirmationToken();
        var tokenExpiration = _emailSenderService.GetEmailConfirmationExpiration();

        var newUser = new ApplicationUser {
            Email = request.Email,
            Name = request.Name,
            Surname = request.Surname,
            RegistrationTime = DateTime.UtcNow,
            LastLoginTime = DateTime.UtcNow,
            Status = UserStatus.Unverified,
            PasswordHash = passwordHash,
            EmailConfirmationToken = token,
            EmailConfirmationExpiration = tokenExpiration
        };

        try
        {
            _userRepository.Add(newUser);

            await _userRepository.SaveChanges(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while registering user");

            return Result.Failure(Errors.DatabaseError);
        }

        await _emailSenderService.SendConfirmationEmail(newUser.Email, getLink(), token, cancellationToken);

        return Result<UserResponse>.Success(
            new UserResponse(
                Name: newUser.Name,
                Surname: newUser.Surname,
                Email: newUser.Email,
                RegistrationTime: newUser.RegistrationTime,
                LastLoginTime: newUser.LastLoginTime,
                Status: newUser.Status
            )
        );
    }
}