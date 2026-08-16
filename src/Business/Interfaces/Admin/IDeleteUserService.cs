using Ardalis.Result;
using Business.Contracts.Admin;

namespace Business.Interfaces.Admin;

public interface IDeleteUserService
{
    Task<Result> DeleteUser(DeleteUserRequest request);
}