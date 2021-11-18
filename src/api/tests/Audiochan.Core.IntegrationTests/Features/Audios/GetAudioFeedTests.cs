using System.Threading.Tasks;
using Audiochan.Core.Audios;
using Audiochan.Core.Audios.Queries;
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
            var artist1 = await RunAsUserAsync(isArtist: true);
            artist1.TryGetUserId(out var artist1Id);
            var audio1 = new AudioFaker(artist1Id).Generate(5);
            InsertRangeIntoDatabase(audio1);
            
            var artist2 = await RunAsUserAsync(isArtist: true);
            artist2.TryGetUserId(out var artistId2);
            var audio2 = new AudioFaker(artistId2).Generate(5);
            InsertRangeIntoDatabase(audio2);
            
            var user = await RunAsDefaultUserAsync();
            user.TryGetUserId(out var userId);
            InsertIntoDatabase(new FollowedArtist
            {
                ObserverId = userId,
                TargetId = artist1Id
            }, new FollowedArtist
            {
                ObserverId = userId,
                TargetId = artistId2
            });

            var result = await SendAsync(new GetAudioFeedQuery(userId));

            result.Items.Should().NotBeEmpty();
            result.Items.Count.Should().Be(10);       
        }
    }
}