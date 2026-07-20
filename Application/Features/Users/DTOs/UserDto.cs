namespace Application.Features.Users.DTOs;

public class UserDto
{
    public int UserId { get; set; }

    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;

    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;

    public int RoleId { get; set; }
    public string Role { get; set; } = string.Empty;

    public int DepartmentId { get; set; }
    public string Department { get; set; } = string.Empty;
    public string? JobTitle { get; set; }

    public bool IsActive { get; set; }

    public DateTime CreatedAt { get; set; }
    public DateTime? LastLoginAt { get; set; }
}
