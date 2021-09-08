using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Audiochan.Core.Auth.Login;
using Audiochan.Core.Auth.Refresh;
using Audiochan.Domain.Entities;
using Bogus;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;

namespace Audiochan.Core.IntegrationTests.Features.Auth
{
    using static TestFixture;

    public class RefreshAccessTokenTests : TestBase
    {
        [Test]
        public async Task ShouldSuccessfullyRefreshAccessToken()
        {
            // Assign
            var faker = new Faker();
            var username = faker.Random.String2(15);
            var password = faker.Internet.Password();
            var (userId, _) = await RunAsUserAsync(username, password);
            
            // Act
            // 1. User logins into account. Receives access and refresh token.
            // 2. In the real world, the access token will expire. Once it expires, then the client needs
            //      to call the refresh token endpoint to get a new access token.
            var loginResult = await SendAsync(new LoginCommand
            {
                Login = username,
                Password = password
            });
            var refreshResult = await SendAsync(new RefreshTokenCommand
            {
                RefreshToken = loginResult.Data!.RefreshToken
            });

            // Assert
            var userRefreshTokens = GetUserRefreshTokens(userId);
            refreshResult.IsSuccess.Should().Be(true);
            refreshResult.Data.Should().NotBeNull();
            userRefreshTokens.Count.Should().BeGreaterThan(0);
            userRefreshTokens.Should().Contain(x => x.Token == refreshResult.Data!.RefreshToken);
            userRefreshTokens.Should().NotContain(x => x.Token == loginResult.Data.RefreshToken);
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