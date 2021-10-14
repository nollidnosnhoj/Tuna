using System.Security.Claims;
using System.Threading.Tasks;
using Audiochan.Core.Auth.GetCurrentUser;
using Audiochan.Core.Common.Interfaces.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;

namespace Audiochan.API.Services
{
    public class CurrentUserService : ICurrentUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IDateTimeProvider _dateTime;

        public CurrentUserService(IHttpContextAccessor httpContextAccessor, IDateTimeProvider dateTimeProvider)
        {
            _httpContextAccessor = httpContextAccessor;
            _dateTime = dateTimeProvider;
        }

        public long GetUserId()
        {
            var value = _httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);
            long.TryParse(value, out long id);
            return id;
        }

        public bool TryGetUserId(out long userId)
        {
            userId = GetUserId();
            return userId > 0;
        }

        public bool TryGetUsername(out string username)
        {
            username = GetUsername();
            return !string.IsNullOrEmpty(username);
        }

        public bool IsAuthenticated()
        {
            return TryGetUserId(out _);
        }

        public string GetUsername()
        {
            return _httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.Name) ?? string.Empty;
        }
    }
}