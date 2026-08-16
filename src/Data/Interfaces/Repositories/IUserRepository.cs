using Data.Entities;

namespace Data.Interfaces.Repositories;

public interface IUserRepository : IBaseRepository<ApplicationUser>
{
    public Task<ApplicationUser?> GetByEmail(string email, CancellationToken cancellationToken = default);
    public Task<ApplicationUser?> GetByEmailConfirmationToken(string token, CancellationToken cancellationToken = default);
}