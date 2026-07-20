using Application.Features.Users.DTOs;
using Domain.Entities.Identity;
using Domain.Repositories;
using MediatR;

namespace Application.Features.Users.Queries.GetUserById;

public class GetUserByIdQueryHandler : IRequestHandler<GetUserByIdQuery, UserDto?>
{
    private readonly IUserRepository _repo;

    public GetUserByIdQueryHandler(IUserRepository repo)
    {
        _repo = repo;
    }

    public async Task<UserDto?> Handle(GetUserByIdQuery request, CancellationToken ct)
    {
        var user = await _repo.GetByIdAsync(request.UserId, ct);
        return user is null ? null : Map(user);
    }

    private static UserDto Map(User user)
    {
        return new UserDto
        {
            UserId = user.UserId,
            Username = user.Username,
            Email = user.Email,
            FirstName = user.FirstName,
            LastName = user.LastName,
            FullName = user.FullName,
            RoleId = user.RoleId,
            Role = user.Role?.Name ?? string.Empty,
            DepartmentId = user.DepartmentId,
            Department = user.Department?.Name ?? string.Empty,
            JobTitle = user.JobTitle,
            IsActive = user.IsActive,
            CreatedAt = user.CreatedAt,
            LastLoginAt = user.LastLoginAt
        };
    }
}
