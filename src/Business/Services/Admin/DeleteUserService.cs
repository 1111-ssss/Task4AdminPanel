using Ardalis.Result;
using Business.Contracts.Admin;
using Business.Interfaces.Admin;
using Data.Interfaces.Repositories;
using FluentValidation;
using Microsoft.Extensions.Logging;

namespace Business.Services.Admin;

public class DeleteUserService : ServiceValidation, IDeleteUserService
{
    private readonly IUserRepository _userRepository;
    private readonly IValidator<UserRequest> _userValidator;
    private readonly ILogger<DeleteUserService> _logger;

    public DeleteUserService(
        IUserRepository userRepository,
        IValidator<UserRequest> userValidator,
        ILogger<DeleteUserService> logger
    )
    {
        _userRepository = userRepository;
        _userValidator = userValidator;
        _logger = logger;
    }

    public async Task<Result> DeleteUser(UserRequest request)
    {
        var validationResult = await Validate(_userValidator, request);
        if (!validationResult.IsSuccess)
        {
            return validationResult;
        }

        var user = await _userRepository.GetByEmail(request.Email);
        if (user is null)
        {
            return Result.NotFound("User not found");
        }

        try
        {
            _userRepository.Delete(user);

            await _userRepository.SaveChanges();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while deleting user");

            return Result.Error("Error while deleting user");
        }

        return Result.Success();
    }
}