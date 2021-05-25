using System;
using System.Threading.Tasks;
using Audiochan.Core.Common.Extensions;
using Audiochan.Core.Features.Auth.Login;
using Audiochan.Core.Features.Auth.Revoke;
using Bogus;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Audiochan.Core.IntegrationTests.Features.Auth
{
    [Collection(nameof(SliceFixture))]
    public class RevokeTokenRequestTests
    {
        private readonly SliceFixture _fixture;

        public RevokeTokenRequestTests(SliceFixture fixture)
        {
            _fixture = fixture;
        }

        [Fact]
        public async Task ShouldSuccessfullyRevokeToken()
        {
            var loginRequestFaker = new Faker<LoginRequest>()
                .RuleFor(x => x.Login, f => f.Name.FirstName().GenerateSlug())
                .RuleFor(x => x.Password, f => f.Internet.Password());

            var loginRequest = loginRequestFaker.Generate();

            var (userId, _) = await _fixture.RunAsUserAsync(loginRequest.Login, loginRequest.Password, Array.Empty<string>());
            
            var loginResult = await _fixture.SendAsync(loginRequest);

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
            var loginRequestFaker = new Faker<LoginRequest>()
                .RuleFor(x => x.Login, f => f.Name.FirstName().GenerateSlug())
                .RuleFor(x => x.Password, f => f.Internet.Password());

            var loginRequest = loginRequestFaker.Generate();

            var (userId, _) = await _fixture.RunAsUserAsync(loginRequest.Login, loginRequest.Password, Array.Empty<string>());
            
            var loginResult1 = await _fixture.SendAsync(loginRequest);
            
            var loginResult2 = await _fixture.SendAsync(new LoginRequest
            {
                Login = loginRequest.Login,
                Password = loginRequest.Password
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