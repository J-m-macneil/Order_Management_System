using Application.Features.Users.DTOs;
using MediatR;

namespace Application.Features.Users.Queries.GetUserById;

public class GetUserByIdQuery : IRequest<UserDto?>
{
    public int UserId { get; set; }
}
