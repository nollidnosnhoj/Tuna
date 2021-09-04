using System;
using System.Linq;
using System.Threading.Tasks;
using Audiochan.Core.Features.Users.SetFollow;
using Audiochan.Domain.Entities;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Audiochan.Core.IntegrationTests.Features.Users
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
            var (targetId, _) = await RunAsUserAsync();
            var (observerId, _) = await RunAsUserAsync();

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
            var (targetId, _) = await RunAsUserAsync();
            var (observerId, _) = await RunAsUserAsync();

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