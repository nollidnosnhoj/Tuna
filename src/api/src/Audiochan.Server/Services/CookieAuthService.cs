using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Audiochan.Application.Features.Users.Models;
using Audiochan.Application.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;

namespace Audiochan.Server.Services;

public class CookieAuthService : IAuthService
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IDateTimeProvider _dateTimeProvider;

    public CookieAuthService(IHttpContextAccessor httpContextAccessor, IDateTimeProvider dateTimeProvider)
    {
        _httpContextAccessor = httpContextAccessor;
        _dateTimeProvider = dateTimeProvider;
    }

    public async Task LoginAsync(UserDto user, CancellationToken cancellationToken = default)
    {
        if (_httpContextAccessor.HttpContext is null) return;
        
        Claim[] claims =
        {
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Name, user.UserName),
            new(ClaimTypes.Email, user.Email)
        };
        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        var principal = new ClaimsPrincipal(identity);
        var properties = new AuthenticationProperties {ExpiresUtc = _dateTimeProvider.Now.AddDays(14)};
        await _httpContextAccessor.HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal, properties);
        _httpContextAccessor.HttpContext.User = principal;
    }

    public async Task LogoutAsync(CancellationToken cancellationToken = default)
    {
        if (_httpContextAccessor.HttpContext is null) return;
        await _httpContextAccessor.HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
    }
}