using Domain.Entities.Identity;
using Domain.Entities.Organisation;

namespace Domain.Repositories;

public interface IUserRepository
{
    Task<int> CountAsync(string? searchTerm, int? roleId, bool? isActive, CancellationToken ct);
    Task<List<User>> GetPagedAsync(string? searchTerm, int? roleId, bool? isActive, int skip, int take, CancellationToken ct);
    Task<User?> GetByIdAsync(int userId, CancellationToken ct);
    Task<bool> UsernameExistsAsync(string username, int? excludingUserId, CancellationToken ct);
    Task<bool> EmailExistsAsync(string email, int? excludingUserId, CancellationToken ct);
    Task<bool> RoleExistsAsync(int roleId, CancellationToken ct);
    Task<bool> DepartmentExistsAsync(int departmentId, CancellationToken ct);
    Task<List<Role>> GetRolesAsync(CancellationToken ct);
    Task<List<Department>> GetDepartmentsAsync(CancellationToken ct);
    Task AddAsync(User user, CancellationToken ct);
    Task SaveChangesAsync(CancellationToken ct);
}
