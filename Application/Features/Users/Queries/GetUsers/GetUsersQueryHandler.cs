using Application.Common.Models;
using Application.Features.Users.DTOs;
using Domain.Entities.Identity;
using Domain.Repositories;
using MediatR;

namespace Application.Features.Users.Queries.GetUsers;

public class GetUsersQueryHandler : IRequestHandler<GetUsersQuery, PagedResult<UserDto>>
{
    private readonly IUserRepository _repo;

    public GetUsersQueryHandler(IUserRepository repo)
    {
        _repo = repo;
    }

    public async Task<PagedResult<UserDto>> Handle(GetUsersQuery request, CancellationToken ct)
    {
        var totalCount = await _repo.CountAsync(request.SearchTerm, request.RoleId, request.IsActive, ct);
        var users = await _repo.GetPagedAsync(request.SearchTerm, request.RoleId, request.IsActive, request.Skip, request.PageSize, ct);

        return new PagedResult<UserDto>
        {
            Items = users.Select(Map).ToList(),
            PageNumber = request.PageNumber,
            PageSize = request.PageSize,
            TotalCount = totalCount
        };
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
