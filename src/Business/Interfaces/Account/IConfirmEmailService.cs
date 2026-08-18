using Business.Common.Result;
using Business.Contracts.Account;

namespace Business.Interfaces.Account;

public interface IConfirmEmailService
{
    Task<Result<UserResponse>> ConfirmEmail(ConfirmEmailRequest request, CancellationToken cancellationToken = default);
}