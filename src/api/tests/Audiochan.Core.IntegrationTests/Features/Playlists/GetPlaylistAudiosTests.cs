using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Audiochan.Core.Features.Audios;
using Audiochan.Core.Features.Audios.GetAudio;
using Audiochan.Core.Features.Playlists.GetPlaylistAudios;
using Audiochan.Domain.Entities;
using Audiochan.Tests.Common.Fakers.Audios;
using Audiochan.Tests.Common.Fakers.Playlists;
using FluentAssertions;
using Xunit;

namespace Audiochan.Core.IntegrationTests.Features.Playlists
{
    public class GetPlaylistAudiosTests : TestBase
    {
        public GetPlaylistAudiosTests(TestFixture testFixture) : base(testFixture)
        {
        }

        [Fact]
        public async Task ShouldSuccessfullyFetchPlaylistAudios()
        {
            var (userId, _) = await RunAsDefaultUserAsync();
            var audioFaker = new AudioFaker(userId);
            var playlistFaker = new PlaylistFaker(userId);
            var audios = audioFaker.Generate(5);
            InsertRangeIntoDatabase(audios);
            var playlist = playlistFaker.Generate();
            InsertIntoDatabase(playlist);
            var playlistAudios = audios.Select(a => new PlaylistAudio
            {
                PlaylistId = playlist.Id,
                AudioId = a.Id
            });
            InsertRangeIntoDatabase(playlistAudios);

            var response = await SendAsync(new GetPlaylistAudiosQuery(playlist.Id));

            response.Should().NotBeNull();
            response.Items.Count.Should().Be(audios.Count);
            response.Items.Should().BeOfType<List<AudioDto>>();
        }
    }
}