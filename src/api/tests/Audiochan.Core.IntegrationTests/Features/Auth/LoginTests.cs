using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Audiochan.Core.Auth.Login;
using Audiochan.Domain.Entities;
using Bogus;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;

namespace Audiochan.Core.IntegrationTests.Features.Auth
{
    using static TestFixture;

    public class LoginTests : TestBase
    {
        [Test]
        public async Task ShouldSuccessfullyLoginAndCreateRefreshToken()
        {
            // Assign
            var faker = new Faker();
            var username = faker.Random.String2(15);
            var password = faker.Internet.Password();
            var (userId, _) = await RunAsUserAsync(username, password);
            
            // Act
            var loginResult = await SendAsync(new LoginCommand
            {
                Login = username,
                Password = password
            });

            // Assert
            var userRefreshTokens = GetUserRefreshTokens(userId);
            
            loginResult.IsSuccess.Should().Be(true);
            loginResult.Data.Should().NotBeNull();
            userRefreshTokens.Count.Should().BeGreaterThan(0);
            userRefreshTokens.Should().Contain(x => x.Token == loginResult.Data!.RefreshToken);
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