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
    private readonly IValidator<DeleteUserRequest> _deleteUserValidator;
    private readonly ILogger<DeleteUserService> _logger;

    public DeleteUserService(
        IUserRepository userRepository,
        IValidator<DeleteUserRequest> deleteUserValidator,
        ILogger<DeleteUserService> logger
    )
    {
        _userRepository = userRepository;
        _deleteUserValidator = deleteUserValidator;
        _logger = logger;
    }

    public async Task<Result> DeleteUser(DeleteUserRequest request)
    {
        var validationResult = await Validate(_deleteUserValidator, request);
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