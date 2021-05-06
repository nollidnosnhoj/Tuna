using System;
using System.Threading.Tasks;
using Audiochan.Core.Features.Auth.Login;
using Audiochan.Core.Features.Auth.Revoke;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Audiochan.IntegrationTests.Features.Auth
{
    [Collection(nameof(SliceFixture))]
    public class RevokeTests
    {
        private readonly SliceFixture _fixture;

        public RevokeTests(SliceFixture fixture)
        {
            _fixture = fixture;
        }

        [Fact]
        public async Task ShouldSuccessfullyRevokeToken()
        {
            var username = "revoketest1";
            var password = "slkdjflksdjflkadsjfkl;dasjflk;ja";

            var (userId, _) = await _fixture.RunAsUserAsync(username, password, Array.Empty<string>());
            
            var loginResult = await _fixture.SendAsync(new LoginRequest
            {
                Login = username,
                Password = password
            });

            var revokeResult = await _fixture.SendAsync(new RevokeTokenRequest
            {
                RefreshToken = loginResult.Data.RefreshToken
            });
            
            var user = await _fixture.ExecuteDbContextAsync(dbContext =>
            {
                return dbContext.Users
                    .Include(u => u.RefreshTokens)
                    .SingleOrDefaultAsync(u => u.Id == userId);
            });
            
            revokeResult.IsSuccess.Should().Be(true);
            user.Should().NotBeNull();
            user.RefreshTokens.Count.Should().Be(0);
        }

        [Fact]
        public async Task ShouldSuccessfullyRevokeOneToken()
        {
            var username = "revoketest2";
            var password = "slkdjflksdjflkadsjfkl;dasjflk;ja";

            var (userId, _) = await _fixture.RunAsUserAsync(username, password, Array.Empty<string>());
            
            var loginResult1 = await _fixture.SendAsync(new LoginRequest
            {
                Login = username,
                Password = password
            });
            
            var loginResult2 = await _fixture.SendAsync(new LoginRequest
            {
                Login = username,
                Password = password
            });

            var revokeResult = await _fixture.SendAsync(new RevokeTokenRequest
            {
                RefreshToken = loginResult2.Data.RefreshToken
            });
            
            var user = await _fixture.ExecuteDbContextAsync(dbContext =>
            {
                return dbContext.Users
                    .Include(u => u.RefreshTokens)
                    .SingleOrDefaultAsync(u => u.Id == userId);
            });
            
            revokeResult.IsSuccess.Should().Be(true);
            user.Should().NotBeNull();
            user.RefreshTokens.Count.Should().Be(1);
            user.RefreshTokens.Should().NotContain(x => x.Token == loginResult2.Data.RefreshToken);
            user.RefreshTokens.Should().Contain(x => x.Token == loginResult1.Data.RefreshToken);
        }
    }
}