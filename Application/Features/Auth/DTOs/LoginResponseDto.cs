using System.Text.Json.Serialization;

namespace Application.Features.Auth.DTOs;

public class LoginResponseDto
{
    [JsonIgnore]
    public string Token { get; set; } = string.Empty;

    [JsonIgnore]
    public string RefreshToken { get; set; } = string.Empty;

    [JsonIgnore]
    public DateTime RefreshTokenExpiresAtUtc { get; set; }

    public DateTime ExpiresAtUtc { get; set; }

    public AuthUserDto User { get; set; } = new();
}
