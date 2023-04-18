using System.Security.Claims;

namespace Audiochan.IntegrationTests;

public static class FakeClaimsPrincipalFactory
{
    public static ClaimsPrincipal Create(string userId)
    {
        return new ClaimsPrincipal(new ClaimsIdentity(new[]
        {
            new Claim(ClaimTypes.NameIdentifier, userId)
        }));
    }
}