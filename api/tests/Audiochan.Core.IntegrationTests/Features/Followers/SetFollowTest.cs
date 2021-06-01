using System;
using System.Threading.Tasks;
using Audiochan.API.Features.Followers.SetFollow;
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
            var (targetId, targetUsername) = await _sliceFixture.RunAsDefaultUserAsync();
            
            var (observerId, _) =
                await _sliceFixture.RunAsUserAsync("kopacetic", "kopacetic123!", Array.Empty<string>());

            await _sliceFixture.SendAsync(new SetFollowRequest(observerId, targetUsername, true));

            var user = await _sliceFixture.ExecuteDbContextAsync(database =>
            {
                return database.Users
                    .AsNoTracking()
                    .Include(u => u.FollowersTable)
                    .SingleOrDefaultAsync(u => u.Id == targetId);
            });

            user.FollowersTable.Should().NotBeEmpty();
            user.FollowersTable.Should().Contain(x => x.ObserverId == observerId && x.TargetId == targetId);
        }
    }
}