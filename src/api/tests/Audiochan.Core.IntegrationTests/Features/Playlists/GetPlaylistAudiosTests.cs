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
    [Collection(nameof(SliceFixture))]
    public class GetPlaylistAudiosTests
    {
        private readonly SliceFixture _sliceFixture;

        public GetPlaylistAudiosTests(SliceFixture sliceFixture)
        {
            _sliceFixture = sliceFixture;
        }

        [Fact]
        public async Task ShouldSuccessfullyFetchPlaylistAudios()
        {
            var (userId, _) = await _sliceFixture.RunAsDefaultUserAsync();
            var audioFaker = new AudioFaker(userId);
            var playlistFaker = new PlaylistFaker(userId);
            var audios = audioFaker.Generate(5);
            await _sliceFixture.InsertRangeAsync(audios);
            var playlist = playlistFaker.Generate();
            foreach (var audio in audios)
            {
                playlist.Audios.Add(new PlaylistAudio
                {
                    AudioId = audio.Id,
                });
            }

            await _sliceFixture.InsertAsync(playlist);

            var response = await _sliceFixture.SendAsync(new GetPlaylistAudiosQuery(playlist.Id));

            response.Should().NotBeNull();
            response.Count.Should().Be(audios.Count);
            response.Items.Count.Should().Be(audios.Count);
            response.Items.Should().BeOfType<List<AudioViewModel>>();
        }
    }
}