using MediatR;

namespace Application.Features.Users.Commands.UpdateUser;

public class UpdateUserCommand : IRequest<Unit>
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string? Password { get; set; }
    public int RoleId { get; set; }
    public int DepartmentId { get; set; }
    public string? JobTitle { get; set; }
    public bool IsActive { get; set; }
}
