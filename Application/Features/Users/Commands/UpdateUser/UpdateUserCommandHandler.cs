using Application.Interfaces;
using Domain.Entities.Identity;
using Domain.Repositories;
using MediatR;

namespace Application.Features.Users.Commands.UpdateUser;

public class UpdateUserCommandHandler : IRequestHandler<UpdateUserRequest, Unit>
{
    private readonly IUserRepository _repo;
    private readonly IPasswordService _passwordService;
    private readonly IAuditService _audit;
    private readonly IAuditChangeFormatter _changeFormatter;

    public UpdateUserCommandHandler(
        IUserRepository repo,
        IPasswordService passwordService,
        IAuditService audit,
        IAuditChangeFormatter changeFormatter)
    {
        _repo = repo;
        _passwordService = passwordService;
        _audit = audit;
        _changeFormatter = changeFormatter;
    }

    public async Task<Unit> Handle(UpdateUserRequest request, CancellationToken ct)
    {
        var dto = request.Data;
        ValidateRequired(dto);

        var user = await _repo.GetByIdAsync(request.UserId, ct);

        if (user is null)
        {
            throw new InvalidOperationException("User not found.");
        }

        var username = dto.Username.Trim();
        var email = dto.Email.Trim();

        if (await _repo.UsernameExistsAsync(username, user.UserId, ct))
        {
            throw new InvalidOperationException("Username is already in use.");
        }

        if (await _repo.EmailExistsAsync(email, user.UserId, ct))
        {
            throw new InvalidOperationException("Email is already in use.");
        }

        await ValidateReferences(dto.RoleId, dto.DepartmentId, ct);

        var oldValues = CreateSnapshot(user);

        user.FirstName = dto.FirstName.Trim();
        user.LastName = dto.LastName.Trim();
        user.FullName = $"{user.FirstName} {user.LastName}";
        user.Email = email;
        user.Username = username;
        user.RoleId = dto.RoleId;
        user.DepartmentId = dto.DepartmentId;
        user.JobTitle = string.IsNullOrWhiteSpace(dto.JobTitle) ? null : dto.JobTitle.Trim();
        user.IsActive = dto.IsActive;

        if (!string.IsNullOrWhiteSpace(dto.Password))
        {
            user.PasswordHash = _passwordService.HashPassword(dto.Password);
        }

        await _repo.SaveChangesAsync(ct);

        var newValues = CreateSnapshot(user);
        var changes = _changeFormatter.GetChanges(oldValues, newValues);

        await _audit.LogAsync(
            "User",
            user.UserId,
            "Updated",
            oldValues,
            newValues,
            _changeFormatter.CreateUpdateNote("User", user.FullName, changes),
            ct);

        return Unit.Value;
    }

    private async Task ValidateReferences(int roleId, int departmentId, CancellationToken ct)
    {
        if (!await _repo.RoleExistsAsync(roleId, ct))
        {
            throw new InvalidOperationException("Selected role does not exist.");
        }

        if (!await _repo.DepartmentExistsAsync(departmentId, ct))
        {
            throw new InvalidOperationException("Selected department does not exist.");
        }
    }

    private static void ValidateRequired(UpdateUserCommand request)
    {
        if (string.IsNullOrWhiteSpace(request.FirstName) ||
            string.IsNullOrWhiteSpace(request.LastName) ||
            string.IsNullOrWhiteSpace(request.Email) ||
            string.IsNullOrWhiteSpace(request.Username))
        {
            throw new InvalidOperationException("First name, last name, email and username are required.");
        }
    }

    private static object CreateSnapshot(User user)
    {
        return new
        {
            user.FirstName,
            user.LastName,
            user.FullName,
            user.Email,
            user.Username,
            user.RoleId,
            user.DepartmentId,
            user.JobTitle,
            user.IsActive
        };
    }
}
