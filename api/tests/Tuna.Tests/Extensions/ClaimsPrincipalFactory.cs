using System.Security.Claims;

namespace Tuna.Tests.Extensions;

public static class ClaimsPrincipalFactory
{
    public static ClaimsPrincipal Create(IEnumerable<Claim> claims)
    {
        return new ClaimsPrincipal(new ClaimsIdentity(claims));
    }
}