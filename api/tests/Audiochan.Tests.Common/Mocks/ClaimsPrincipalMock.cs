using System.Security.Claims;
using Bogus;

namespace Audiochan.Tests.Common.Mocks;

public static class ClaimsPrincipalMock
{
    public static ClaimsPrincipal CreateFakeUser(Randomizer? randomizer = null)
    {
        randomizer ??= new Randomizer();
        Claim[] claims = {
            new(ClaimTypes.NameIdentifier, randomizer.Long(min: 0).ToString()),
            new(ClaimTypes.Name, randomizer.String2(7)),
        };
        var identity = new ClaimsIdentity(claims, "Test");
        return new ClaimsPrincipal(identity);
    }
}