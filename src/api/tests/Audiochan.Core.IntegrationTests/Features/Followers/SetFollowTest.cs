using System;
using System.Threading.Tasks;
using Audiochan.Core.Entities;
using Audiochan.Core.Features.Followers.SetFollow;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Audiochan.Core.IntegrationTests.Features.Followers
{
    [Collection(nameof(SliceFixture))]
    public class SetFollowTest
    {
        private readonly SliceFixture _sliceFixture;

        public SetFollowTest(SliceFixture sliceFixture)
        {
            _sliceFixture = sliceFixture;
        }

        [Fact]
        public async Task AddFollowerTest()
        {
            // Assign
            var (targetId, _) = await _sliceFixture.RunAsDefaultUserAsync();
            var (observerId, _) =
                await _sliceFixture.RunAsUserAsync("kopacetic", "kopacetic123!", Array.Empty<string>());

            // Act
            await _sliceFixture.SendAsync(new SetFollowCommand(observerId, targetId, true));

            var user = await GetUsersWithFollowers(targetId);

            // Assert
            user.Followers.Should().NotBeEmpty();
            user.Followers.Should().Contain(x => x.ObserverId == observerId && x.TargetId == targetId);
        }

        [Fact]
        public async Task RemoveFollowerTest()
        {
            // Assign
            var (targetId, _) = await _sliceFixture.RunAsDefaultUserAsync();
            var (observerId, _) =
                await _sliceFixture.RunAsUserAsync("kopacetic", "kopacetic123!", Array.Empty<string>());

            var user = await GetUsersWithFollowers(targetId);

            await UpdateUserWithNewFollower(user, observerId);

            // Act
            await _sliceFixture.SendAsync(new SetFollowCommand(observerId, targetId, false));

            user = await GetUsersWithFollowers(targetId);

            // Assert
            user.Followers.Should().BeEmpty();
        }

        private async Task<User> GetUsersWithFollowers(string userId)
        {
            return await _sliceFixture.ExecuteDbContextAsync(db =>
            {
                return db.Users.Include(u => u.Followers)
                    .SingleOrDefaultAsync(u => u.Id == userId);
            });
        }

        private async Task UpdateUserWithNewFollower(User user, string observerId)
        {
            user.Followers.Add(new FollowedUser
            {
                ObserverId = observerId,
                FollowedDate = new DateTime(2021, 1, 1)
            });
            
            await _sliceFixture.ExecuteDbContextAsync(db =>
            {
                db.Users.Update(user);
                return db.SaveChangesAsync();
            });
        }
    }
}