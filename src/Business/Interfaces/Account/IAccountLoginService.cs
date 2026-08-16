using Ardalis.Result;
using Business.Contracts.Account;

namespace Business.Interfaces.Account;

public interface IAccountLoginService
{
    Task<Result<UserResponse>> Login(LoginUserRequest request, Func<string> getLink);
}