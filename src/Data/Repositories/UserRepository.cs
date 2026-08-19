using Data.Database;
using Data.Database.Exceptions;
using Data.Entities;
using Data.Enums;
using Data.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;
using Npgsql;

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

    public async Task AddUser(ApplicationUser user, CancellationToken cancellationToken = default)
    {
        _dbSet.Add(user);

        try
        {
            await _context.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateException ex) when (IsUniqueEmailViolation(ex))
        {
            throw new DuplicateEmailException(user.Email, ex);
        }
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

    public async Task<UserStatus?> GetUserStatusByEmail(string email, CancellationToken cancellationToken = default)
    {
        var user = await _dbSet
            .FirstOrDefaultAsync(u => u.Email == email, cancellationToken);

        return user?.Status;
    }

    private IQueryable<ApplicationUser> ApplyUserSorting(IQueryable<ApplicationUser> query, string orderBy, bool isAsc)
    {
        return orderBy.ToLower() switch
        {
            "email" => isAsc ? query.OrderBy(u => u.Email) : query.OrderByDescending(u => u.Email),
            "registrationtime" => isAsc ? query.OrderBy(u => u.RegistrationTime) : query.OrderByDescending(u => u.RegistrationTime),
            "lastlogintime" => isAsc ? query.OrderBy(u => u.LastLoginTime) : query.OrderByDescending(u => u.LastLoginTime),
            "status" => isAsc ? query.OrderBy(u => u.Status) : query.OrderByDescending(u => u.Status),
            _ => isAsc ? query.OrderBy(u => u.Surname).ThenBy(u => u.Name) : query.OrderByDescending(u => u.Surname).ThenByDescending(u => u.Name)
        };
    }

    private static bool IsUniqueEmailViolation(DbUpdateException ex)
    {
        if (ex.InnerException is PostgresException pg)
        {
            return pg.SqlState == PostgresErrorCodes.UniqueViolation
                && pg.ConstraintName == "ix_user_email";
        }

        // Fallback
        var message = ex.InnerException?.Message ?? ex.Message;
        return message.Contains("ix_user_email", StringComparison.OrdinalIgnoreCase)
            || message.Contains("23505");
    }
}