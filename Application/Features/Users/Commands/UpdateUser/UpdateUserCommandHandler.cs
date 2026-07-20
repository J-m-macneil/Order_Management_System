using Application.Common.Exceptions;
using Application.Common.Validation;
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

    public UpdateUserCommandHandler(
        IUserRepository repo,
        IPasswordService passwordService,
        IAuditService audit)
    {
        _repo = repo;
        _passwordService = passwordService;
        _audit = audit;
    }

    public async Task<Unit> Handle(UpdateUserRequest request, CancellationToken ct)
    {
        var dto = request.Data;
        ValidateRequest(dto);

        var user = await _repo.GetByIdAsync(request.UserId, ct);

        if (user is null)
        {
            throw new NotFoundException("User", request.UserId);
        }

        var username = dto.Username.Trim();
        var email = dto.Email.Trim();

        if (await _repo.UsernameExistsAsync(username, user.UserId, ct))
        {
            throw new ConflictException("Username is already in use.");
        }

        if (await _repo.EmailExistsAsync(email, user.UserId, ct))
        {
            throw new ConflictException("Email is already in use.");
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

        await _audit.LogAsync(
            "User",
            user.UserId,
            "Updated",
            oldValues,
            newValues,
            $"User updated: {user.FullName}.",
            ct);

        return Unit.Value;
    }

    private async Task ValidateReferences(int roleId, int departmentId, CancellationToken ct)
    {
        if (!await _repo.RoleExistsAsync(roleId, ct))
        {
            throw new BadRequestException("Selected role does not exist.");
        }

        if (!await _repo.DepartmentExistsAsync(departmentId, ct))
        {
            throw new BadRequestException("Selected department does not exist.");
        }
    }

    private static void ValidateRequest(UpdateUserCommand request)
    {
        CommandValidation.RequiredText(request.FirstName, "First name", 80);
        CommandValidation.RequiredText(request.LastName, "Last name", 80);
        CommandValidation.Email(request.Email);
        CommandValidation.RequiredText(request.Username, "Username", 50);
        CommandValidation.OptionalText(request.JobTitle, "Job title", 120);
        CommandValidation.PositiveId(request.RoleId, "Role");
        CommandValidation.PositiveId(request.DepartmentId, "Department");

        if (!string.IsNullOrWhiteSpace(request.Password))
        {
            CommandValidation.MinimumLength(request.Password, "Password", 8);
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
