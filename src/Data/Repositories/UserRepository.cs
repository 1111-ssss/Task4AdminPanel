using Data.Database;
using Data.Entities;
using Data.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Data.Repositories;

public class UserRepository : BaseRepository<ApplicationUser>, IUserRepository
{
    public UserRepository(AppDbContext context) : base(context) { }

    public async Task<ApplicationUser?> GetByEmail(string email, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .FirstOrDefaultAsync(u => u.Email == email, cancellationToken);
    }

    public async Task<ApplicationUser?> GetByEmailConfirmationToken(string token, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .FirstOrDefaultAsync(u => u.EmailConfirmationToken == token, cancellationToken);
    }

    public async Task<(IEnumerable<ApplicationUser>, int totalCount)> ListUsers(
        int page,
        int pageSize,
        string orderBy,
        string? search,
        bool? isAsc,
        CancellationToken cancellationToken = default
    )
    {
        var query = _context.Users.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(search))
        {
            string lowerSearch = search.ToLower();
            query = query.Where(u => u.Email!.ToLower().Contains(lowerSearch)
                || u.Name!.ToLower().Contains(lowerSearch)
                || u.Surname!.ToLower().Contains(lowerSearch));
        }

        int totalCount = await query.CountAsync(cancellationToken);

        query = ApplyUserSorting(query, orderBy, isAsc ?? true);

        var users = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (users, totalCount);
    }

    private IQueryable<ApplicationUser> ApplyUserSorting(IQueryable<ApplicationUser> query, string orderBy, bool isAsc)
    {
        return orderBy.ToLower() switch
        {
            "email" => isAsc ? query.OrderBy(u => u.Email) : query.OrderByDescending(u => u.Email),
            "surname" => isAsc ? query.OrderBy(u => u.Surname) : query.OrderByDescending(u => u.Surname),
            "registrationtime" => isAsc ? query.OrderBy(u => u.RegistrationTime) : query.OrderByDescending(u => u.RegistrationTime),
            "lastlogintime" => isAsc ? query.OrderBy(u => u.LastLoginTime) : query.OrderByDescending(u => u.LastLoginTime),
            "status" => isAsc ? query.OrderBy(u => u.Status) : query.OrderByDescending(u => u.Status),
            _ => isAsc ? query.OrderBy(u => u.Name) : query.OrderByDescending(u => u.Name)
        };
    }
}