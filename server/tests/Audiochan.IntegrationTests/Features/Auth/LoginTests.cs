using System;
using System.Linq;
using System.Threading.Tasks;
using Audiochan.Features.Auth.Login;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Audiochan.IntegrationTests.Features.Auth
{
    [Collection(nameof(SliceFixture))]
    public class LoginTests
    {
        private readonly SliceFixture _fixture;

        public LoginTests(SliceFixture fixture)
        {
            _fixture = fixture;
        }

        [Fact]
        public async Task ShouldSuccessfullyLoginAndCreateRefreshToken()
        {
            var username = "logintestusername";
            var password = "loginTestPassword#@@@@";
            
            var (userId, _) = await _fixture.RunAsUserAsync(username, password, Array.Empty<string>());
            
            var loginResult = await _fixture.SendAsync(new LoginRequest
            {
                Login = username,
                Password = password
            });
            
            var user = await _fixture.ExecuteDbContextAsync(dbContext =>
            {
                return dbContext.Users
                    .Include(u => u.RefreshTokens)
                    .SingleOrDefaultAsync(u => u.Id == userId);
            });

            loginResult.IsSuccess.Should().Be(true);
            loginResult.Data.Should().NotBeNull();
            user.Should().NotBeNull();
            user.RefreshTokens.Count.Should().BeGreaterThan(0);
            user.RefreshTokens.Should().Contain(x => x.Token == loginResult.Data.RefreshToken);
        }
    }
}