using System;
using System.Linq;
using System.Threading.Tasks;
using Audiochan.Core.Entities;
using Audiochan.Core.Features.Followers.SetFollow;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Audiochan.Core.IntegrationTests.Features.Followers
{
    public class SetFollowTest : TestBase
    {
        public SetFollowTest(TestFixture testFixture) : base(testFixture)
        {
        }

        [Fact]
        public async Task AddFollowerTest()
        {
            // Assign
            var (targetId, _) = await RunAsDefaultUserAsync();
            var (observerId, _) = await RunAsUserAsync("kopacetic");

            // Act
            await SendAsync(new SetFollowCommand(observerId, targetId, true));

            var user = GetUsersWithFollowers(targetId);

            // Assert
            user.Should().NotBeNull();
            user!.Followers.Should().NotBeEmpty();
            user.Followers.Should().Contain(x => x.ObserverId == observerId && x.TargetId == targetId);
        }

        [Fact]
        public async Task RemoveFollowerTest()
        {
            // Assign
            var (targetId, _) = await RunAsDefaultUserAsync();
            var (observerId, _) = await RunAsUserAsync("kopacetic");

            var user = GetUsersWithFollowers(targetId);

            user.Should().NotBeNull("Did not run as user");

            UpdateUserWithNewFollower(user!, observerId);

            // Act
            await SendAsync(new SetFollowCommand(observerId, targetId, false));

            user = GetUsersWithFollowers(targetId);

            // Assert
            user.Should().NotBeNull();
            user!.Followers.Should().BeEmpty();
        }

        private User? GetUsersWithFollowers(long userId)
        {
            return ExecuteDbContext(db =>
            {
                return db.Users.Include(u => u.Followers)
                    .SingleOrDefault(u => u.Id == userId);
            });
        }

        private void UpdateUserWithNewFollower(User user, long observerId)
        {
            user.Followers.Add(new FollowedUser
            {
                ObserverId = observerId,
                FollowedDate = new DateTime(2021, 1, 1)
            });
            
            ExecuteDbContext(db =>
            {
                db.Users.Update(user);
                db.SaveChanges();
            });
        }
    }
}