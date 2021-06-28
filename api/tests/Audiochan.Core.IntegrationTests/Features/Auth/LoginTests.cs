using System;
using System.Threading.Tasks;
using Audiochan.Core.Features.Auth.Login;
using Bogus;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Audiochan.Core.IntegrationTests.Features.Auth
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
            // Assign
            var faker = new Faker();
            var username = faker.Random.String2(15);
            var password = faker.Internet.Password();
            var (userId, _) = await _fixture.RunAsUserAsync(username, password);
            
            // Act
            var loginResult = await _fixture.SendAsync(new LoginCommand
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

            // Assert
            loginResult.IsSuccess.Should().Be(true);
            loginResult.Data.Should().NotBeNull();
            user.Should().NotBeNull();
            user.RefreshTokens.Count.Should().BeGreaterThan(0);
            user.RefreshTokens.Should().Contain(x => x.Token == loginResult.Data!.RefreshToken);
        }
    }
}