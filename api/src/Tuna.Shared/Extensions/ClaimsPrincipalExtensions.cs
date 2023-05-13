using System.Security.Claims;
using Microsoft.IdentityModel.JsonWebTokens;

namespace Tuna.Shared.Extensions;

public static class ClaimNames
{
    public const string UserId = "userId";
    public const string HasProfile = "hasProfile";
}

public static class ClaimsPrincipalExtensions
{
    public static bool TryGetUserId(this ClaimsPrincipal? principal, out long userId)
    {
        var claim = principal?.FindFirst(ClaimNames.UserId);

        if (claim is not null) return long.TryParse(claim.Value, out userId);

        userId = 0;
        return false;
    }

    public static bool TryGetUserName(this ClaimsPrincipal? principal, out string userName)
    {
        var claim = principal?.FindFirst(JwtRegisteredClaimNames.Name);

        if (claim is not null && !string.IsNullOrWhiteSpace(claim.Value))
        {
            userName = claim.Value;
            return true;
        }

        userName = "";
        return false;
    }

    public static long GetUserId(this ClaimsPrincipal? principal)
    {
        if (!principal.TryGetUserId(out var userId)) throw new UnauthorizedAccessException();

        return userId;
    }

    public static string GetUserName(this ClaimsPrincipal? principal)
    {
        if (!principal.TryGetUserName(out var userName)) throw new UnauthorizedAccessException();

        return userName;
    }
}