using System.Security.Claims;
using Audiochan.Core.Entities;
using Audiochan.Core.Features.Auth.Helpers;
using Bogus;

namespace Audiochan.IntegrationTests.Extensions;

public static class TestFixtureExtensions
{
    public static async Task<(User user, ClaimsPrincipal principal)> CreateRandomUserAsync(this TestFixture fixture, Faker? faker = null)
    {
        faker ??= new Faker();
        var user = new User(faker.Random.Guid().ToString(), faker.Internet.UserName());
        await fixture.InsertAsync(user);
        var claims = UserClaimsHelpers.ToClaims(faker.Random.Guid().ToString(), user);
        var claimPrincipal = ClaimsPrincipalFactory.Create(claims);
        return (user, claimPrincipal);
    }
}