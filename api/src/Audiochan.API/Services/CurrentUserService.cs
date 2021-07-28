using System.Security.Claims;
using Audiochan.Core.Common.Interfaces;
using Audiochan.Core.Interfaces;
using Microsoft.AspNetCore.Http;

namespace Audiochan.API.Services
{
    public class CurrentUserService : ICurrentUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CurrentUserService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public string GetUserId()
        {
            return _httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier)
                   ?? string.Empty;
        }

        public bool TryGetUserId(out string userId)
        {
            userId = GetUserId();
            return !string.IsNullOrEmpty(userId);
        }

        public bool TryGetUsername(out string username)
        {
            username = GetUsername();
            return !string.IsNullOrEmpty(username);
        }

        public bool IsAuthenticated()
        {
            var id = GetUserId();
            return !string.IsNullOrEmpty(id);
        }

        public string GetUsername()
        {
            return _httpContextAccessor.HttpContext?.User.FindFirstValue("name") ?? string.Empty;
        }
    }
}