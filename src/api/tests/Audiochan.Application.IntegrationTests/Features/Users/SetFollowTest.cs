using System;
using System.Linq;
using System.Threading.Tasks;
using Audiochan.Application.Commons.Extensions;
using Audiochan.Application.Features.Users.Commands.SetFollow;
using Audiochan.Domain.Entities;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;

namespace Audiochan.Application.IntegrationTests.Features.Users
{
    using static TestFixture;

    public class SetFollowTest : TestBase
    {
        [Test]
        public async Task AddFollowerTest()
        {
            // Assign
            var target = await RunAsUserAsync();
            target.TryGetUserId(out var targetId);
            var observer = await RunAsUserAsync();
            observer.TryGetUserId(out var observerId);

            // Act
            await SendAsync(new SetFollowCommand(observerId, targetId, true));

            var user = GetUsersWithFollowers(targetId);

            // Assert
            user.Should().NotBeNull();
            user!.Followers.Should().NotBeEmpty();
            user.Followers.Should().Contain(x => x.ObserverId == observerId && x.TargetId == targetId);
        }

        [Test]
        public async Task RemoveFollowerTest()
        {
            // Assign
            var target = await RunAsUserAsync();
            target.TryGetUserId(out var targetId);
            var observer = await RunAsUserAsync();
            observer.TryGetUserId(out var observerId);
            
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
                FollowedDate = new DateTime(2021, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            });
            
            ExecuteDbContext(db =>
            {
                db.Users.Update(user);
                db.SaveChanges();
            });
        }
    }
}