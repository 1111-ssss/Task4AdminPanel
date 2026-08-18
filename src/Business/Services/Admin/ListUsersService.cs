using Business.Common.Result;
using Business.Contracts.Account;
using Business.Contracts.Admin;
using Business.Interfaces.Admin;
using Data.Interfaces.Repositories;
using FluentValidation;
using Microsoft.Extensions.Logging;

namespace Business.Services.Admin;

public class ListUsersService : ServiceValidation, IListUsersService
{
    private readonly IUserRepository _userRepository;
    private readonly IValidator<ListUsersRequest> _listUsersValidator;
    private readonly ILogger<ListUsersService> _logger;

    public ListUsersService(
        IUserRepository userRepository,
        IValidator<ListUsersRequest> listUsersValidator,
        ILogger<ListUsersService> logger
    )
    {
        _userRepository = userRepository;
        _listUsersValidator = listUsersValidator;
        _logger = logger;
    }

    public async Task<Result<ListUsersResponse>> ListUsers(ListUsersRequest request, CancellationToken cancellationToken)
    {
        var validationResult = await Validate(_listUsersValidator, request, cancellationToken);
        if (!validationResult.IsSuccess)
        {
            return validationResult;
        }

        var (users, totalCount) = await _userRepository.ListUsers(
            request.Page,
            request.PageSize,
            request.OrderBy,
            request.Search,
            request.IsAsc,
            cancellationToken
        );

        var userResponses = users.Select(u => new UserResponse(
            Name: u.Name,
            Surname: u.Surname,
            Email: u.Email,
            RegistrationTime: u.RegistrationTime,
            LastLoginTime: u.LastLoginTime,
            Status: u.Status
        )).ToList();

        return new Result<ListUsersResponse>(
            new ListUsersResponse(
                request.Page,
                request.PageSize,
                (long)Math.Ceiling((double)totalCount / request.PageSize),
                totalCount,
                userResponses
            )
        );
    }
}