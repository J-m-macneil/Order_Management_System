using Application.Common.Interfaces;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace Infrastructure.Identity;

public class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUserService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public int? UserId
    {
        get
        {
            var userId = _httpContextAccessor.HttpContext?.User?
                .FindFirst(ClaimTypes.NameIdentifier)?.Value;

            return int.TryParse(userId, out var id) ? id : null;
        }
    }

    public List<string> Roles
    {
        get
        {
            return _httpContextAccessor.HttpContext?.User?
                .Claims
                .Where(c =>
                    c.Type == ClaimTypes.Role ||
                    c.Type == "role")
                .Select(c => c.Value)
                .ToList()
                ?? new List<string>();
        }
    }
}