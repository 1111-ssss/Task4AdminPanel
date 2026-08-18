using Business.Common.Result;
using Business.Contracts.Admin;

namespace Business.Interfaces.Admin;

public interface IBlockUserService
{
    Task<Result> BlockUser(UserRequest request, CancellationToken cancellationToken);
    Task<Result> UnblockUser(UserRequest request, CancellationToken cancellationToken);
}