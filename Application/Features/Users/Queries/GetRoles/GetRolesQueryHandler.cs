using Application.Features.Users.DTOs;
using Domain.Repositories;
using MediatR;

namespace Application.Features.Users.Queries.GetRoles;

public class GetRolesQueryHandler : IRequestHandler<GetRolesQuery, List<RoleDto>>
{
    private readonly IUserRepository _repo;

    public GetRolesQueryHandler(IUserRepository repo)
    {
        _repo = repo;
    }

    public async Task<List<RoleDto>> Handle(GetRolesQuery request, CancellationToken ct)
    {
        var roles = await _repo.GetRolesAsync(ct);

        return roles.Select(x => new RoleDto
        {
            RoleId = x.RoleId,
            Name = x.Name
        }).ToList();
    }
}
