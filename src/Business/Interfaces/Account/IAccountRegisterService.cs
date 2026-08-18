using Business.Common.Result;
using Business.Contracts.Account;

namespace Business.Interfaces.Account;

public interface IAccountRegisterService
{
    Task<Result<UserResponse>> Register(RegisterUserRequest request, Func<string> getLink, CancellationToken cancellationToken);
}