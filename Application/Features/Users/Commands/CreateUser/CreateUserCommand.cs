using Application.Features.Users.DTOs;
using MediatR;

namespace Application.Features.Users.Commands.CreateUser;

public class CreateUserCommand : IRequest<UserDto>
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public int RoleId { get; set; }
    public int DepartmentId { get; set; }
    public string? JobTitle { get; set; }
    public bool IsActive { get; set; } = true;
}
