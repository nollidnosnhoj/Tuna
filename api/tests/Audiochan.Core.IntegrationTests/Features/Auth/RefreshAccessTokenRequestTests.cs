using System;
using System.Threading.Tasks;
using Audiochan.Core.Common.Extensions;
using Audiochan.Core.Features.Auth.Login;
using Audiochan.Core.Features.Auth.Refresh;
using Bogus;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Audiochan.Core.IntegrationTests.Features.Auth
{
    [Collection(nameof(SliceFixture))]
    public class RefreshAccessTokenRequestTests
    {
        private readonly SliceFixture _fixture;

        public RefreshAccessTokenRequestTests(SliceFixture fixture)
        {
            _fixture = fixture;
        }

        [Fact]
        public async Task ShouldSuccessfullyRefreshAccessToken()
        {
            var loginRequestFaker = new Faker<LoginRequest>()
                .RuleFor(x => x.Login, f => f.Name.FirstName().GenerateSlug())
                .RuleFor(x => x.Password, f => f.Internet.Password());

            var loginRequest = loginRequestFaker.Generate();

            var (userId, _) = await _fixture.RunAsUserAsync(loginRequest.Login, loginRequest.Password, Array.Empty<string>());
            
            var loginResult = await _fixture.SendAsync(loginRequest);

            var refreshResult = await _fixture.SendAsync(new RefreshTokenRequest
            {
                RefreshToken = loginResult.Data.RefreshToken
            });
            
            var user = await _fixture.ExecuteDbContextAsync(dbContext =>
            {
                return dbContext.Users
                    .Include(u => u.RefreshTokens)
                    .SingleOrDefaultAsync(u => u.Id == userId);
            });

            refreshResult.IsSuccess.Should().Be(true);
            refreshResult.Data.Should().NotBeNull();
            user.Should().NotBeNull();
            user.RefreshTokens.Count.Should().BeGreaterThan(0);
            user.RefreshTokens.Should().Contain(x => x.Token == refreshResult.Data.RefreshToken);
            user.RefreshTokens.Should().NotContain(x => x.Token == loginResult.Data.RefreshToken);
        }
    }
}