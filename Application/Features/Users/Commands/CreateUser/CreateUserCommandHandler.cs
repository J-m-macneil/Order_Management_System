using Application.Features.Users.DTOs;
using Application.Interfaces;
using Domain.Entities.Identity;
using Domain.Repositories;
using MediatR;

namespace Application.Features.Users.Commands.CreateUser;

public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, UserDto>
{
    private readonly IUserRepository _repo;
    private readonly IPasswordService _passwordService;
    private readonly IAuditService _audit;

    public CreateUserCommandHandler(
        IUserRepository repo,
        IPasswordService passwordService,
        IAuditService audit)
    {
        _repo = repo;
        _passwordService = passwordService;
        _audit = audit;
    }

    public async Task<UserDto> Handle(CreateUserCommand request, CancellationToken ct)
    {
        ValidateRequired(request);

        var username = request.Username.Trim();
        var email = request.Email.Trim();

        if (await _repo.UsernameExistsAsync(username, null, ct))
        {
            throw new InvalidOperationException("Username is already in use.");
        }

        if (await _repo.EmailExistsAsync(email, null, ct))
        {
            throw new InvalidOperationException("Email is already in use.");
        }

        await ValidateReferences(request.RoleId, request.DepartmentId, ct);

        var user = new User
        {
            FirstName = request.FirstName.Trim(),
            LastName = request.LastName.Trim(),
            FullName = $"{request.FirstName.Trim()} {request.LastName.Trim()}",
            Email = email,
            Username = username,
            PasswordHash = _passwordService.HashPassword(request.Password),
            RoleId = request.RoleId,
            DepartmentId = request.DepartmentId,
            JobTitle = string.IsNullOrWhiteSpace(request.JobTitle) ? null : request.JobTitle.Trim(),
            IsActive = request.IsActive,
            CreatedAt = DateTime.UtcNow
        };

        await _repo.AddAsync(user, ct);

        var newValues = CreateSnapshot(user);

        await _audit.LogAsync(
            "User",
            user.UserId,
            "Created",
            null,
            newValues,
            $"User created: {user.FullName}.",
            ct);

        return new UserDto
        {
            UserId = user.UserId,
            Username = user.Username,
            Email = user.Email,
            FirstName = user.FirstName,
            LastName = user.LastName,
            FullName = user.FullName,
            RoleId = user.RoleId,
            DepartmentId = user.DepartmentId,
            JobTitle = user.JobTitle,
            IsActive = user.IsActive,
            CreatedAt = user.CreatedAt,
            LastLoginAt = user.LastLoginAt
        };
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

    private static void ValidateRequired(CreateUserCommand request)
    {
        if (string.IsNullOrWhiteSpace(request.FirstName) ||
            string.IsNullOrWhiteSpace(request.LastName) ||
            string.IsNullOrWhiteSpace(request.Email) ||
            string.IsNullOrWhiteSpace(request.Username) ||
            string.IsNullOrWhiteSpace(request.Password))
        {
            throw new InvalidOperationException("First name, last name, email, username and password are required.");
        }
    }

    private static object CreateSnapshot(User user)
    {
        return new
        {
            user.UserId,
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
