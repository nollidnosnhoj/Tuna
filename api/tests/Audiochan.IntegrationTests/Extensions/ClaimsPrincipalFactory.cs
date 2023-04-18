using System.Security.Claims;

namespace Audiochan.IntegrationTests.Extensions;

public static class ClaimsPrincipalFactory
{
    public static ClaimsPrincipal Create(IEnumerable<Claim> claims)
    {
        return new ClaimsPrincipal(new ClaimsIdentity(claims));
    }
}