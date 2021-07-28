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
    [Collection(nameof(SliceFixture))]
    public class GetPlaylistDetailsTests
    {
        private readonly SliceFixture _sliceFixture;

        public GetPlaylistDetailsTests(SliceFixture sliceFixture)
        {
            _sliceFixture = sliceFixture;
        }

        [Fact]
        public async Task ShouldSuccessfullyFetchPlaylist()
        {
            var (userId, _) = await _sliceFixture.RunAsDefaultUserAsync();
            var audioFaker = new AudioFaker(userId);
            var audios = audioFaker.Generate(5);
            var playlist = new PlaylistFaker(userId).Generate();
            await _sliceFixture.InsertRangeAsync(audios);
            await _sliceFixture.InsertAsync(playlist);
            var now = DateTime.UtcNow;
            var playlistAudios = audios
                .Select((t, i) => new PlaylistAudio
                {
                    PlaylistId = playlist.Id,
                    AudioId = t.Id,
                    Added = now.AddMinutes((audios.Count - i) * -1)
                }).ToList();
            await _sliceFixture.InsertRangeAsync(playlistAudios);

            var response = await _sliceFixture
                .SendAsync(new GetPlaylistDetailQuery(playlist.Id, true));

            response.Should().NotBeNull();
            response.Should().BeOfType<PlaylistDetailViewModel>();
            response!.Title.Should().Be(playlist.Title);
            response.Description.Should().Be(playlist.Description);
            response.Visibility.Should().Be(playlist.Visibility);
            response.Picture.Should().BeNullOrEmpty();
            response.Audios.Should().NotBeEmpty();
            response.Audios.Count.Should().Be(audios.Count);
        }
    }
}