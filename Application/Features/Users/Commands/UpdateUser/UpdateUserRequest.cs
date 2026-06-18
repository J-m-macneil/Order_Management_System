using MediatR;

namespace Application.Features.Users.Commands.UpdateUser;

public class UpdateUserRequest : IRequest<Unit>
{
    public int UserId { get; set; }
    public UpdateUserCommand Data { get; set; } = new();
}
