using System.Security.Claims;
using Audiochan.Core.Interfaces;
using Microsoft.AspNetCore.Http;

namespace Audiochan.Web.Services
{
    public class CurrentUserService : ICurrentUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CurrentUserService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public long GetUserId()
        {
            var x = _httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);

            return !long.TryParse(x, out var id) ? 0 : id;
        }

        public bool IsAuthenticated(long? userId = null)
        {
            userId ??= GetUserId();

            return userId > 0;
        }

        public string GetUsername()
        {
            return _httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.Name) ?? "";
        }
    }
}