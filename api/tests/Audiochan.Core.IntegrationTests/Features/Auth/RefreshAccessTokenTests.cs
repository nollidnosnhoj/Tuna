using System;
using System.Threading.Tasks;
using Audiochan.Core.Features.Auth.Login;
using Audiochan.Core.Features.Auth.Refresh;
using Bogus;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Audiochan.Core.IntegrationTests.Features.Auth
{
    [Collection(nameof(SliceFixture))]
    public class RefreshAccessTokenTests
    {
        private readonly SliceFixture _fixture;

        public RefreshAccessTokenTests(SliceFixture fixture)
        {
            _fixture = fixture;
        }

        [Fact]
        public async Task ShouldSuccessfullyRefreshAccessToken()
        {
            // Assign
            var faker = new Faker();
            var username = faker.Random.String2(15);
            var password = faker.Internet.Password();
            var (userId, _) = await _fixture.RunAsUserAsync(username, password, Array.Empty<string>());
            
            // Act
            var loginResult = await _fixture.SendAsync(new LoginCommand
            {
                Login = username,
                Password = password
            });
            var refreshResult = await _fixture.SendAsync(new RefreshTokenCommand
            {
                RefreshToken = loginResult.Data!.RefreshToken
            });
            var user = await _fixture.ExecuteDbContextAsync(dbContext =>
            {
                return dbContext.Users
                    .Include(u => u.RefreshTokens)
                    .SingleOrDefaultAsync(u => u.Id == userId);
            });

            // Assert
            refreshResult.IsSuccess.Should().Be(true);
            refreshResult.Data.Should().NotBeNull();
            user.Should().NotBeNull();
            user.RefreshTokens.Count.Should().BeGreaterThan(0);
            user.RefreshTokens.Should().Contain(x => x.Token == refreshResult.Data!.RefreshToken);
            user.RefreshTokens.Should().NotContain(x => x.Token == loginResult.Data.RefreshToken);
        }
    }
}