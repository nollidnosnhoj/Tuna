using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Audiochan.Core.Auth.Login;
using Audiochan.Core.Auth.Revoke;
using Audiochan.Domain.Entities;
using Bogus;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;

namespace Audiochan.Core.IntegrationTests.Features.Auth
{
    using static TestFixture;

    public class RevokeTokenTests : TestBase
    {
        [Test]
        public async Task ShouldSuccessfullyRevokeToken()
        {
            // Assign
            var faker = new Faker();
            var username = faker.Random.String2(15);
            var password = faker.Internet.Password();
            var (userId, _) = await RunAsUserAsync(username, password);
            
            // Act
            // When the user logins, the user can revoke their refresh token. Meaning they cannot refresh their access
            // token.
            var loginResult = await SendAsync(new LoginCommand
            {
                Login = username,
                Password = password
            });
            var revokeResult = await SendAsync(new RevokeTokenCommand
            {
                RefreshToken = loginResult.Data!.RefreshToken
            });

            // Assert
            var userRefreshTokens = GetUserRefreshTokens(userId);
            revokeResult.IsSuccess.Should().Be(true);
            userRefreshTokens.Count.Should().Be(0);
        }

        [Test]
        public async Task ShouldSuccessfullyRevokeOneToken()
        {
            // Assign
            var faker = new Faker();
            var username = faker.Random.String2(15);
            var password = faker.Internet.Password();
            var (userId, _) = await RunAsUserAsync(username, password);
            
            // Act
            // When the user logins into multiple session, each session has their own refresh token. This
            // is to simulate the various sessions.
            var command = new LoginCommand
            {
                Login = username,
                Password = password
            };
            var loginResult1 = await SendAsync(command);
            var loginResult2 = await SendAsync(command);
            var revokeResult = await SendAsync(new RevokeTokenCommand
            {
                RefreshToken = loginResult2.Data!.RefreshToken
            });

            // Assert
            var userRefreshTokens = GetUserRefreshTokens(userId);
            revokeResult.IsSuccess.Should().Be(true);
            userRefreshTokens.Count.Should().Be(1);
            userRefreshTokens.Should().NotContain(x => x.Token == loginResult2.Data.RefreshToken);
            userRefreshTokens.Should().Contain(x => x.Token == loginResult1.Data!.RefreshToken);
        }
        
        private List<RefreshToken> GetUserRefreshTokens(long userId)
        {
            return ExecuteDbContext(dbContext =>
            {
                return dbContext.Users
                    .AsNoTracking()
                    .Include(u => u.RefreshTokens)
                    .Where(u => u.Id == userId)
                    .SelectMany(u => u.RefreshTokens)
                    .ToList();
            });
        }
    }
}