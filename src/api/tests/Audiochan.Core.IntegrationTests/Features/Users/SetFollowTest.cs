using System;
using System.Linq;
using System.Threading.Tasks;
using Audiochan.Core.Artists.Commands;
using Audiochan.Core.Common.Extensions;
using Audiochan.Domain.Entities;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;

namespace Audiochan.Core.IntegrationTests.Features.Users
{
    using static TestFixture;

    public class SetFollowTest : TestBase
    {
        [Test]
        public async Task AddFollowerTest()
        {
            // Assign
            var target = await RunAsUserAsync(isArtist: true);
            target.TryGetUserId(out var targetId);
            var observer = await RunAsUserAsync();
            observer.TryGetUserId(out var observerId);

            // Act
            await SendAsync(new SetFollowCommand(observerId, targetId, true));

            var user = GetArtistWithFollowers(targetId);

            // Assert
            user.Should().NotBeNull();
            user!.Followers.Should().NotBeEmpty();
            user.Followers.Should().Contain(x => x.ObserverId == observerId && x.TargetId == targetId);
        }

        [Test]
        public async Task RemoveFollowerTest()
        {
            // Assign
            var target = await RunAsUserAsync(isArtist: true);
            target.TryGetUserId(out var targetId);
            var observer = await RunAsUserAsync();
            observer.TryGetUserId(out var observerId);
            
            var user = GetArtistWithFollowers(targetId);

            user.Should().NotBeNull("Did not run as user");

            UpdateArtistWithNewFollower(user!, observerId);

            // Act
            await SendAsync(new SetFollowCommand(observerId, targetId, false));

            user = GetArtistWithFollowers(targetId);

            // Assert
            user.Should().NotBeNull();
            user!.Followers.Should().BeEmpty();
        }

        private Artist? GetArtistWithFollowers(long artistId)
        {
            return ExecuteDbContext(db =>
            {
                return db.Artists.Include(u => u.Followers)
                    .SingleOrDefault(u => u.Id == artistId);
            });
        }

        private void UpdateArtistWithNewFollower(Artist user, long observerId)
        {
            user.Followers.Add(new FollowedArtist
            {
                ObserverId = observerId,
                FollowedDate = new DateTime(2021, 1, 1)
            });
            
            ExecuteDbContext(db =>
            {
                db.SaveChanges();
            });
        }
    }
}