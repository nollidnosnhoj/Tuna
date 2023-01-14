using System;
using System.Net.Http;
using System.Threading.Tasks;
using Audiochan.Core.IntegrationTests.Extensions;
using Audiochan.Core.Persistence;
using Audiochan.Domain.Entities;
using Audiochan.Tests.Common.Fakers.Users;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Audiochan.Core.IntegrationTests.Features.Users
{
    public class SetFollowTest
    {
        private readonly AudiochanApiFactory _factory;
        public SetFollowTest(AudiochanApiFactory factory)
        {
            _factory = factory;
        }
        
        [Fact]
        public async Task AddFollowerTest()
        {
            using var scope = _factory.Services.CreateScope();
            await using var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            
            var users = new UserFaker().Generate(2);
            var target = users[0];
            var observer = users[1];

            await dbContext.Users.AddRangeAsync(target, observer);
            await dbContext.SaveChangesAsync();

            // Act
            using var client = _factory.CreateClientWithTestAuth()
                .AddUserIdToAuthorization(observer.Id);
            using var request = new HttpRequestMessage(HttpMethod.Put, $"/me/followings/{target.Id}");
            using var response = await client.SendAsync(request);

            var user = await GetUsersWithFollowers(dbContext, target.Id);

            // Assert
            user.Should().NotBeNull();
            user!.Followers.Should().NotBeEmpty();
            user.Followers.Should().Contain(x => x.ObserverId == observer.Id && x.TargetId == target.Id);
        }

        [Fact]
        public async Task RemoveFollowerTest()
        {
            // Assign
            using var scope = _factory.Services.CreateScope();
            await using var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            
            var users = new UserFaker().Generate(2);
            var target = users[0];
            var observer = users[1];

            await dbContext.Users.AddRangeAsync(target, observer);
            await dbContext.SaveChangesAsync();
            
            var user = await GetUsersWithFollowers(dbContext, target.Id);

            user.Should().NotBeNull("Did not run as user");

            await UpdateUserWithNewFollower(dbContext, user!, observer.Id);

            // Act
            using var client = _factory.CreateClientWithTestAuth()
                .AddUserIdToAuthorization(observer.Id);
            using var request = new HttpRequestMessage(HttpMethod.Delete, $"/me/followings/{target.Id}");
            using var response = await client.SendAsync(request);

            user = await GetUsersWithFollowers(dbContext, target.Id);

            // Assert
            user.Should().NotBeNull();
            user!.Followers.Should().BeEmpty();
        }

        private async Task<User?> GetUsersWithFollowers(ApplicationDbContext dbContext, long userId)
        {
            return await dbContext.Users
                .Include(u => u.Followers)
                .SingleOrDefaultAsync(u => u.Id == userId);
        }

        private async Task UpdateUserWithNewFollower(ApplicationDbContext dbContext, User user, long observerId)
        {
            user.Followers.Add(new FollowedUser
            {
                ObserverId = observerId,
                FollowedDate = new DateTime(2021, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            });
            
            dbContext.Users.Update(user);
            await dbContext.SaveChangesAsync();
        }
    }
}