using Application.Features.Users.DTOs;
using MediatR;

namespace Application.Features.Users.Queries.GetRoles;

public class GetRolesQuery : IRequest<List<RoleDto>>
{
}
