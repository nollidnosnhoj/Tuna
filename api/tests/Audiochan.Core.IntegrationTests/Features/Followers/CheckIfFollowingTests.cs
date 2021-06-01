using System;
using System.Threading.Tasks;
using Audiochan.API.Features.Followers.CheckIfFollowing;
using FluentAssertions;
using Xunit;

namespace Audiochan.Core.IntegrationTests.Features.Followers
{
    [Collection(nameof(SliceFixture))]
    public class CheckIfFollowingTests
    {
        private readonly SliceFixture _sliceFixture;

        public CheckIfFollowingTests(SliceFixture sliceFixture)
        {
            _sliceFixture = sliceFixture;
        }

        [Fact]
        public async Task ShouldReturnTrue_WhenUserIsFollowing()
        {
            var (targetId, targetUsername) = await _sliceFixture.RunAsAdministratorAsync();
            var (observerId, _) = await _sliceFixture.RunAsUserAsync("followuser1", "test", Array.Empty<string>());
            var favoriteAudio = new Entities.FollowedUser
            {
                TargetId = targetId,
                ObserverId = observerId,
                FollowedDate = DateTime.Now
            };
            await _sliceFixture.InsertAsync(favoriteAudio);

            var isFollowing = await _sliceFixture.SendAsync(new CheckIfUserIsFollowingRequest(observerId, targetUsername));

            isFollowing.Should().BeTrue();
        }
        
        [Fact]
        public async Task ShouldReturnFalse_WhenUserDidNotFollowing()
        {
            var (_, targetUsername) = await _sliceFixture.RunAsAdministratorAsync();
            var (observerId, _) = await _sliceFixture.RunAsUserAsync("followuser2", "test", Array.Empty<string>());
            var isFollowing = await _sliceFixture.SendAsync(new CheckIfUserIsFollowingRequest(observerId, targetUsername));

            isFollowing.Should().BeFalse();
        }
        
        [Fact]
        public async Task ShouldReturnFalse_WhenUserUnfollowed()
        {
            var (targetId, targetUsername) = await _sliceFixture.RunAsAdministratorAsync();
            var (observerId, _) = await _sliceFixture.RunAsUserAsync("followuser3", "test", Array.Empty<string>());
            var now = DateTime.UtcNow;
            var favoriteAudio = new Entities.FollowedUser
            {
                TargetId = targetId,
                ObserverId = observerId,
                FollowedDate = now.AddDays(-1),
                UnfollowedDate = now
            };
            await _sliceFixture.InsertAsync(favoriteAudio);

            var isFollowing = await _sliceFixture.SendAsync(new CheckIfUserIsFollowingRequest(observerId, targetUsername));

            isFollowing.Should().BeFalse();
        }
    }
}