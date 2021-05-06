using System.Security.Claims;
using Audiochan.Core.Common.Interfaces;
using Microsoft.AspNetCore.Http;

namespace Audiochan.Services
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

        public bool IsAuthenticated()
        {
            var id = GetUserId();
            return !string.IsNullOrEmpty(id);
        }

        public string GetUsername()
        {
            return _httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.Name) ?? string.Empty;
        }
    }
}