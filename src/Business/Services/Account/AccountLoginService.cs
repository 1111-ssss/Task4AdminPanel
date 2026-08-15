using Ardalis.Result;
using Business.Contracts.Account;
using Business.Interfaces.Account;
using Data.Interfaces.Repositories;
using Data.Interfaces.Services;
using FluentValidation;
using Microsoft.Extensions.Logging;

namespace Business.Services.Account;

public class AccountLoginService : ServiceValidation, IAccountLoginService
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IValidator<LoginUserRequest> _loginUserValidator;
    private readonly ILogger<AccountLoginService> _logger;

    public AccountLoginService(
        IUserRepository userRepository,
        IPasswordHasher passwordHasher,
        IValidator<LoginUserRequest> loginUserValidator,
        ILogger<AccountLoginService> logger
    )
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _loginUserValidator = loginUserValidator;
        _logger = logger;
    }

    public async Task<Result<UserResponse>> Login(LoginUserRequest request)
    {
        var user = await _userRepository.GetByEmail(request.Email);
        if (user is null)
        {
            return Result.NotFound("User with this email does not exist");
        }

        var validationResult = await Validate(_loginUserValidator, request);
        if (!validationResult.IsSuccess)
        {
            return validationResult;
        }

        if (!_passwordHasher.VerifyPassword(request.Password, user.PasswordHash))
        {
            return Result.Unauthorized("Invalid password");
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

            return Result.Error("Error while updating user last login time");
        }

        return Result.Success(
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