using System;
using System.Threading.Tasks;
using Audiochan.Features.Auth.Login;
using Audiochan.Features.Auth.Refresh;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Audiochan.IntegrationTests.Features.Auth
{
    [Collection(nameof(SliceFixture))]
    public class RefreshTests
    {
        private readonly SliceFixture _fixture;

        public RefreshTests(SliceFixture fixture)
        {
            _fixture = fixture;
        }

        [Fact]
        public async Task ShouldSuccessfullyRefreshAccessToken()
        {
            var username = "refreshtokentestuser";
            var password = "refreshtokentestPassword#@@@@";

            var (userId, _) = await _fixture.RunAsUserAsync(username, password, Array.Empty<string>());
            
            var loginResult = await _fixture.SendAsync(new LoginRequest
            {
                Login = username,
                Password = password
            });

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