namespace Business.Contracts.Admin;

public record ListUsersRequest(
    int Page,
    int PageSize,
    string OrderBy,
    string? Search,
    bool? IsAsc
);