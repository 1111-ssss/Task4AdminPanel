using Data.Interfaces.Repositories;
using Business.Contracts.Account;
using Business.Interfaces.Account;
using Data.Interfaces.Services;
using Microsoft.Extensions.Logging;
using Ardalis.Result;
using FluentValidation;
using Data.Entities;
using Data.Enums;

namespace Business.Services.Account;

public class AccountRegisterService : ServiceValidation, IAccountRegisterService
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IValidator<RegisterUserRequest> _registerUserValidator;
    private readonly ILogger<AccountRegisterService> _logger;

    public AccountRegisterService(
        IUserRepository userRepository,
        IPasswordHasher passwordHasher,
        IValidator<RegisterUserRequest> registerUserValidator,
        ILogger<AccountRegisterService> logger
    )
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _registerUserValidator = registerUserValidator;
        _logger = logger;
    }
    
    public async Task<Result<UserResponse>> Register(RegisterUserRequest request)
    {
        var user = await _userRepository.GetByEmail(request.Email);
        if (user is not null)
        {
            return Result.Conflict("User with this email already exists");
        }

        var validationResult = await Validate(_registerUserValidator, request);
        if (!validationResult.IsSuccess)
        {
            return validationResult;
        }

        var passwordHash = _passwordHasher.HashPassword(request.Password);

        var newUser = new ApplicationUser {
            Email = request.Email,
            Name = request.Name,
            Surname = request.Surname,
            RegistrationTime = DateTime.UtcNow,
            LastLoginTime = DateTime.UtcNow,
            Status = UserStatus.Unverified,
            PasswordHash = passwordHash,
            EmailConfirmationToken = Guid.NewGuid().ToString(),
            EmailConfirmationExpiration = DateTime.UtcNow.AddHours(24)
        };

        try
        {
            _userRepository.Add(newUser);

            await _userRepository.SaveChanges();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while registering user");

            return Result.Error("Error while registering user");
        }

        return Result.Success(
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