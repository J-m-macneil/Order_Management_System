using Domain.Entities.Identity;

namespace Application.Interfaces;

public interface IJwtTokenService
{
    (string Token, DateTime ExpiresAtUtc) GenerateToken(User user, string roleName);
}