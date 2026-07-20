using Domain.Entities.Identity;
using Domain.Entities.Organisation;
using Domain.Repositories;
using Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories;

public class UserRepository : IUserRepository
{
    private readonly AppDbContext _db;

    public UserRepository(AppDbContext db)
    {
        _db = db;
    }

    public Task<int> CountAsync(string? searchTerm, int? roleId, bool? isActive, CancellationToken ct)
    {
        return ApplyFilters(_db.Users.AsNoTracking(), searchTerm, roleId, isActive).CountAsync(ct);
    }

    public Task<List<User>> GetPagedAsync(string? searchTerm, int? roleId, bool? isActive, int skip, int take, CancellationToken ct)
    {
        return ApplyFilters(_db.Users.AsNoTracking(), searchTerm, roleId, isActive)
            .Include(x => x.Role)
            .Include(x => x.Department)
            .OrderBy(x => x.LastName)
            .ThenBy(x => x.FirstName)
            .Skip(skip)
            .Take(take)
            .ToListAsync(ct);
    }

    public Task<User?> GetByIdAsync(int userId, CancellationToken ct)
    {
        return _db.Users
            .Include(x => x.Role)
            .Include(x => x.Department)
            .FirstOrDefaultAsync(x => x.UserId == userId, ct);
    }

    public Task<bool> UsernameExistsAsync(string username, int? excludingUserId, CancellationToken ct)
    {
        var normalized = username.Trim();

        return _db.Users.AnyAsync(x =>
            x.Username == normalized &&
            (!excludingUserId.HasValue || x.UserId != excludingUserId.Value), ct);
    }

    public Task<bool> EmailExistsAsync(string email, int? excludingUserId, CancellationToken ct)
    {
        var normalized = email.Trim();

        return _db.Users.AnyAsync(x =>
            x.Email == normalized &&
            (!excludingUserId.HasValue || x.UserId != excludingUserId.Value), ct);
    }

    public Task<bool> RoleExistsAsync(int roleId, CancellationToken ct)
    {
        return _db.Roles.AnyAsync(x => x.RoleId == roleId, ct);
    }

    public Task<bool> DepartmentExistsAsync(int departmentId, CancellationToken ct)
    {
        return _db.Departments.AnyAsync(x => x.DepartmentId == departmentId, ct);
    }

    public Task<List<Role>> GetRolesAsync(CancellationToken ct)
    {
        return _db.Roles
            .AsNoTracking()
            .OrderBy(x => x.Name)
            .ToListAsync(ct);
    }

    public Task<List<Department>> GetDepartmentsAsync(CancellationToken ct)
    {
        return _db.Departments
            .AsNoTracking()
            .OrderBy(x => x.Name)
            .ToListAsync(ct);
    }

    public async Task AddAsync(User user, CancellationToken ct)
    {
        await _db.Users.AddAsync(user, ct);
        await _db.SaveChangesAsync(ct);
    }

    public Task SaveChangesAsync(CancellationToken ct)
    {
        return _db.SaveChangesAsync(ct);
    }

    private static IQueryable<User> ApplyFilters(IQueryable<User> query, string? searchTerm, int? roleId, bool? isActive)
    {
        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var pattern = $"%{searchTerm.Trim()}%";
            query = query.Where(x =>
                EF.Functions.ILike(x.FirstName, pattern) ||
                EF.Functions.ILike(x.LastName, pattern) ||
                EF.Functions.ILike(x.FullName, pattern) ||
                EF.Functions.ILike(x.Username, pattern) ||
                EF.Functions.ILike(x.Email, pattern));
        }

        if (roleId.HasValue)
        {
            query = query.Where(x => x.RoleId == roleId.Value);
        }

        if (isActive.HasValue)
        {
            query = query.Where(x => x.IsActive == isActive.Value);
        }

        return query;
    }
}
