using System.Threading.Tasks;
using Audiochan.Core.Features.Audios.GetAudioFeed;
using Audiochan.Domain.Entities;
using Audiochan.Tests.Common.Fakers.Audios;
using FluentAssertions;
using JetBrains.Annotations;
using Xunit;

namespace Audiochan.Core.IntegrationTests.Features.Audios
{
    public class GetAudioFeedTests : TestBase
    {
        public GetAudioFeedTests(TestFixture fixture) : base(fixture)
        {
        }

        [Fact]
        public async Task SuccessfullyGetAudioFeed()
        {
            var (oneId, _) = await RunAsUserAsync();
            var oneAudios = new AudioFaker(oneId).Generate(5);
            InsertRangeIntoDatabase(oneAudios);
            
            var (twoId, _) = await RunAsUserAsync();
            var twoAudios = new AudioFaker(twoId).Generate(5);
            InsertRangeIntoDatabase(twoAudios);
            
            var (userId, _) = await RunAsDefaultUserAsync();
            InsertIntoDatabase(new FollowedUser
            {
                ObserverId = userId,
                TargetId = oneId
            }, new FollowedUser
            {
                ObserverId = userId,
                TargetId = twoId
            });

            var result = await SendAsync(new GetAudioFeedQuery(userId));

            result.Items.Should().NotBeEmpty();
            result.Items.Count.Should().Be(10);
        }
    }
}