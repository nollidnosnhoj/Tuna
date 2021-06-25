using System;
using System.Threading.Tasks;
using Audiochan.Core.Features.Auth.Login;
using Audiochan.Core.Features.Auth.Revoke;
using Bogus;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Audiochan.Core.IntegrationTests.Features.Auth
{
    [Collection(nameof(SliceFixture))]
    public class RevokeTokenTests
    {
        private readonly SliceFixture _fixture;

        public RevokeTokenTests(SliceFixture fixture)
        {
            _fixture = fixture;
        }

        [Fact]
        public async Task ShouldSuccessfullyRevokeToken()
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
            var revokeResult = await _fixture.SendAsync(new RevokeTokenCommand
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
            revokeResult.IsSuccess.Should().Be(true);
            user.Should().NotBeNull();
            user.RefreshTokens.Count.Should().Be(0);
        }

        [Fact]
        public async Task ShouldSuccessfullyRevokeOneToken()
        {
            // Assign
            var faker = new Faker();
            var username = faker.Random.String2(15);
            var password = faker.Internet.Password();
            var (userId, _) = await _fixture.RunAsUserAsync(username, password, Array.Empty<string>());
            
            // Act
            var command = new LoginCommand
            {
                Login = username,
                Password = password
            };
            var loginResult1 = await _fixture.SendAsync(command);
            var loginResult2 = await _fixture.SendAsync(command);
            var revokeResult = await _fixture.SendAsync(new RevokeTokenCommand
            {
                RefreshToken = loginResult2.Data!.RefreshToken
            });
            
            var user = await _fixture.ExecuteDbContextAsync(dbContext =>
            {
                return dbContext.Users
                    .Include(u => u.RefreshTokens)
                    .SingleOrDefaultAsync(u => u.Id == userId);
            });
            
            // Assert
            revokeResult.IsSuccess.Should().Be(true);
            user.Should().NotBeNull();
            user.RefreshTokens.Count.Should().Be(1);
            user.RefreshTokens.Should().NotContain(x => x.Token == loginResult2.Data.RefreshToken);
            user.RefreshTokens.Should().Contain(x => x.Token == loginResult1.Data!.RefreshToken);
        }
    }
}