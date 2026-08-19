using Business.Common.Errors;
using Business.Common.Result;
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

    public async Task<Result> BlockUser(UserRequest request, CancellationToken cancellationToken)
    {
        var validationResult = await Validate(_userValidator, request, cancellationToken);
        if (!validationResult.IsSuccess)
        {
            return validationResult;
        }

        var user = await _userRepository.GetByEmail(request.Email, cancellationToken);
        if (user is null)
        {
            return Result.Failure(Errors.UserNotFound);
        }

        if (user.Status == UserStatus.Blocked)
        {
            return Result.Failure(Errors.UserAlreadyBlocked);
        }

        try
        {
            user.Status = UserStatus.Blocked;
            _userRepository.Update(user);

            await _userRepository.SaveChanges(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while blocking user");

            return Result.Failure(Errors.DatabaseError);
        }

        return Result.Success();
    }

    public async Task<Result> UnblockUser(UserRequest request, CancellationToken cancellationToken)
    {
        var validationResult = await Validate(_userValidator, request, cancellationToken);
        if (!validationResult.IsSuccess)
        {
            return validationResult;
        }

        var user = await _userRepository.GetByEmail(request.Email, cancellationToken);
        if (user is null)
        {
            return Result.Failure(Errors.UserNotFound);
        }

        if (user.Status != UserStatus.Blocked)
        {
            return Result.Failure(Errors.UserNotBlocked);
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

            await _userRepository.SaveChanges(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while unblocking user");

            return Result.Failure(Errors.DatabaseError);
        }

        return Result.Success();
    }
}