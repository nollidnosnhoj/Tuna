using System.Collections.Generic;
using System.Threading.Tasks;
using Audiochan.Core.Entities;
using Audiochan.Core.Features.Audios.GetAudio;
using Audiochan.Core.Features.Playlists.GetPlaylistAudios;
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
            InsertRange(audios);
            var playlist = playlistFaker.Generate();
            foreach (var audio in audios)
            {
                playlist.Audios.Add(new PlaylistAudio
                {
                    AudioId = audio.Id,
                });
            }

            Insert(playlist);

            var response = await SendAsync(new GetPlaylistAudiosQuery(playlist.Id));

            response.Should().NotBeNull();
            response.Count.Should().Be(audios.Count);
            response.Items.Count.Should().Be(audios.Count);
            response.Items.Should().BeOfType<List<AudioViewModel>>();
        }
    }
}