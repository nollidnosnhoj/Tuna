using System.Threading.Tasks;
using Audiochan.Core.Audios;
using Audiochan.Core.Common.Extensions;
using Audiochan.Domain.Entities;
using Audiochan.Tests.Common.Fakers.Audios;
using FluentAssertions;
using NUnit.Framework;

namespace Audiochan.Core.IntegrationTests.Features.Audios
{
    using static TestFixture;
    
    public class GetAudioFeedTests : TestBase
    {
        [Test]
        public async Task SuccessfullyGetAudioFeed()
        {
            var user1 = await RunAsUserAsync();
            user1.TryGetUserId(out var user1Id);
            var oneAudios = new AudioFaker(user1Id).Generate(5);
            InsertRangeIntoDatabase(oneAudios);
            
            var user2 = await RunAsUserAsync();
            user2.TryGetUserId(out var user2Id);
            var twoAudios = new AudioFaker(user2Id).Generate(5);
            InsertRangeIntoDatabase(twoAudios);
            
            var user = await RunAsDefaultUserAsync();
            user.TryGetUserId(out var userId);
            InsertIntoDatabase(new FollowedUser
            {
                ObserverId = userId,
                TargetId = user1Id
            }, new FollowedUser
            {
                ObserverId = userId,
                TargetId = user2Id
            });

            var result = await SendAsync(new GetAudioFeedQuery(userId));

            result.Items.Should().NotBeEmpty();
            result.Items.Count.Should().Be(10);       
        }
    }
}