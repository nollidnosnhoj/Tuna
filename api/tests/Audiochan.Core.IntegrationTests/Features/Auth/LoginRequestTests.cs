using System;
using System.Threading.Tasks;
using Audiochan.Core.Common.Extensions;
using Audiochan.Core.Features.Auth.Login;
using Bogus;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Audiochan.Core.IntegrationTests.Features.Auth
{
    [Collection(nameof(SliceFixture))]
    public class LoginRequestTests
    {
        private readonly SliceFixture _fixture;

        public LoginRequestTests(SliceFixture fixture)
        {
            _fixture = fixture;
        }

        [Fact]
        public async Task ShouldSuccessfullyLoginAndCreateRefreshToken()
        {
            var loginRequestFaker = new Faker<LoginRequest>()
                .RuleFor(x => x.Login, f => f.Name.FirstName().GenerateSlug())
                .RuleFor(x => x.Password, f => f.Internet.Password());

            var request = loginRequestFaker.Generate();
            
            var (userId, _) = await _fixture.RunAsUserAsync(request.Login, request.Password, Array.Empty<string>());
            
            var loginResult = await _fixture.SendAsync(request);
            
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