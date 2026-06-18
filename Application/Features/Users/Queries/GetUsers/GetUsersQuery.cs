using Application.Common.Models;
using Application.Features.Users.DTOs;
using MediatR;

namespace Application.Features.Users.Queries.GetUsers;

public class GetUsersQuery : PaginationQuery, IRequest<PagedResult<UserDto>>
{
    public string? SearchTerm { get; set; }
    public int? RoleId { get; set; }
    public bool? IsActive { get; set; }
}
