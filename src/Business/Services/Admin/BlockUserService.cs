using Ardalis.Result;
using Business.Contracts.Admin;
using Business.Interfaces.Admin;
using Data.Enums;
using Data.Interfaces.Repositories;
using FluentValidation;
using Microsoft.Extensions.Logging;

namespace Business.Services.Admin;

public class BlockUserService : ServiceValidation, IBlockUserService
{
    private readonly IUserRepository _userRepository;
    private readonly IValidator<UserRequest> _userValidator;
    private readonly ILogger<BlockUserService> _logger;

    public BlockUserService(
        IUserRepository userRepository,
        IValidator<UserRequest> userValidator,
        ILogger<BlockUserService> logger
    )
    {
        _userRepository = userRepository;
        _userValidator = userValidator;
        _logger = logger;
    }

    public async Task<Result> BlockUser(UserRequest request)
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

        if (user.Status == UserStatus.Blocked)
        {
            return Result.Conflict("User is already blocked");
        }

        try
        {
            user.Status = UserStatus.Blocked;
            _userRepository.Update(user);

            await _userRepository.SaveChanges();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while blocking user");

            return Result.Error("Error while blocking user");
        }

        return Result.Success();
    }

    public async Task<Result> UnblockUser(UserRequest request)
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

        if (user.Status != UserStatus.Blocked)
        {
            return Result.Conflict("User is not blocked");
        }

        try
        {
            if (user.EmailConfirmationToken is not null)
            {
                user.Status = UserStatus.Unverified;
            }
            else
            {
                user.Status = UserStatus.Active;
            }
            _userRepository.Update(user);

            await _userRepository.SaveChanges();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while unblocking user");

            return Result.Error("Error while unblocking user");
        }

        return Result.Success();
    }
}