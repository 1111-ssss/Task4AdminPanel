using Data.Entities;

namespace Data.Interfaces.Repositories;

public interface IUserRepository : IBaseRepository<ApplicationUser>
{
    public Task<ApplicationUser?> GetByEmail(string email, CancellationToken cancellationToken = default);
}