using Business.Contracts.Account;

namespace Business.Contracts.Admin;

public record ListUsersResponse(
    int Page,
    int PageSize,
    long TotalPages,
    long TotalRecords,
    List<UserResponse> Users
);