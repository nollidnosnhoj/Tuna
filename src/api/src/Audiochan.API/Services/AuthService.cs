using System.Security.Claims;
using System.Threading.Tasks;
using Audiochan.Core.Auth.GetCurrentUser;
using Audiochan.Core.Common.Interfaces.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;

namespace Audiochan.API.Services
{
    public class AuthService : IAuthService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IDateTimeProvider _dateTime;

        public AuthService(IHttpContextAccessor httpContextAccessor, IDateTimeProvider dateTimeProvider)
        {
            _httpContextAccessor = httpContextAccessor;
            _dateTime = dateTimeProvider;
        }

        public async Task SignIn(CurrentUserDto user)
        {
            var httpContext = _httpContextAccessor.HttpContext;
            Claim[] claims = {
                new(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new(ClaimTypes.Name, user.UserName),
                new(ClaimTypes.Email, user.Email)
            };
            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);
            var properties = new AuthenticationProperties { ExpiresUtc = _dateTime.Now.AddDays(14) };
            await httpContext!.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal, properties);
            httpContext!.User = principal;
        }

        public async Task SignOut()
        {
            var httpContext = _httpContextAccessor.HttpContext;
            await httpContext!.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
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
            return _httpContextAccessor.HttpContext?.User.FindFirstValue("name") ?? string.Empty;
        }
    }
}