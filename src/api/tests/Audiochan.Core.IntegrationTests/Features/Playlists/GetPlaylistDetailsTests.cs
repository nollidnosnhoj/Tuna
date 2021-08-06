using System;
using System.Linq;
using System.Threading.Tasks;
using Audiochan.Core.Entities;
using Audiochan.Core.Features.Playlists.GetPlaylistDetail;
using Audiochan.Tests.Common.Fakers.Audios;
using Audiochan.Tests.Common.Fakers.Playlists;
using FluentAssertions;
using Xunit;

namespace Audiochan.Core.IntegrationTests.Features.Playlists
{
    public class GetPlaylistDetailsTests : TestBase
    {
        public GetPlaylistDetailsTests(TestFixture testFixture) : base(testFixture)
        {
        }

        [Fact]
        public async Task ShouldSuccessfullyFetchPlaylist()
        {
            var (userId, _) = await RunAsDefaultUserAsync();
            var audioFaker = new AudioFaker(userId);
            var audios = audioFaker.Generate(5);
            var playlist = new PlaylistFaker(userId).Generate();
            InsertRange(audios);
            Insert(playlist);
            var now = DateTime.UtcNow;
            var playlistAudios = audios
                .Select((t, i) => new PlaylistAudio
                {
                    PlaylistId = playlist.Id,
                    AudioId = t.Id,
                }).ToList();
            InsertRange(playlistAudios);

            var response = await SendAsync(new GetPlaylistDetailQuery(playlist.Id));

            response.Should().NotBeNull();
            response.Should().BeOfType<PlaylistViewModel>();
            response!.Title.Should().Be(playlist.Title);
            response.Description.Should().Be(playlist.Description);
            response.Visibility.Should().Be(playlist.Visibility);
            response.Picture.Should().BeNullOrEmpty();
        }
    }
}