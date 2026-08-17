using Data.Common.Result;
using Business.Contracts.Admin;

namespace Business.Interfaces.Admin;

public interface IListUsersService
{
    Task<Result<ListUsersResponse>> ListUsers(ListUsersRequest request);
}