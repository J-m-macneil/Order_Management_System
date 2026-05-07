using Application.Common.Interfaces;
using System.Security.Claims;

namespace Server.Server.Services
{
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
                var userId = _httpContextAccessor.HttpContext?
                    .User?
                    .FindFirst(ClaimTypes.NameIdentifier)?
                    .Value;

                return int.TryParse(userId, out var id) ? id : null;
            }
        }

        public List<string> Roles
        {
            get
            {
                return _httpContextAccessor.HttpContext?
                    .User?
                    .Claims
                    .Where(c => c.Type == ClaimTypes.Role)
                    .Select(c => c.Value)
                    .ToList()
                    ?? new List<string>();
            }
        }
    }
}
