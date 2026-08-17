using Data.Common.Result;
using Business.Contracts.Admin;

namespace Business.Interfaces.Admin;

public interface IBlockUserService
{
    Task<Result> BlockUser(UserRequest request);
    Task<Result> UnblockUser(UserRequest request);
}