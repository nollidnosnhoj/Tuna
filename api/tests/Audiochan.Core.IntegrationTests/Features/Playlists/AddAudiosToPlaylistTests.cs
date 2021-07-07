using System.Linq;
using System.Threading.Tasks;
using Audiochan.Core.Common.Models;
using Audiochan.Core.Features.Playlists.AddAudiosToPlaylist;
using Audiochan.Tests.Common.Fakers.Audios;
using Audiochan.Tests.Common.Fakers.Playlists;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Audiochan.Core.IntegrationTests.Features.Playlists
{
    [Collection(nameof(SliceFixture))]
    public class AddAudiosToPlaylistTests
    {
        private readonly SliceFixture _sliceFixture;

        public AddAudiosToPlaylistTests(SliceFixture sliceFixture)
        {
            _sliceFixture = sliceFixture;
        }

        [Fact]
        public async Task ShouldSuccessfullyAddAudiosToPlaylist()
        {
            var (userId, _) = await _sliceFixture.RunAsUserAsync("kopacetic");
            var playlistFaker = new PlaylistFaker(userId);
            var playlist = playlistFaker.Generate();
            await _sliceFixture.InsertAsync(playlist);

            var audioFaker = new AudioFaker(userId);
            var audios = audioFaker.Generate(3);
            var audioIds = audios.Select(a => a.Id).ToList();
            await _sliceFixture.InsertRangeAsync(audios);

            var request = new AddAudiosToPlaylistCommand(playlist.Id, audioIds);
            var result = await _sliceFixture.SendAsync(request);
            var loadedPlaylist = await _sliceFixture.ExecuteDbContextAsync(db =>
            {
                return db.Playlists
                    .Include(a => a.Audios)
                    .Where(p => p.Id == playlist.Id).SingleOrDefaultAsync();
            });

            result.IsSuccess.Should().BeTrue();
            loadedPlaylist.Audios.Count.Should().Be(audioIds.Count);
            loadedPlaylist.Audios.Should().Contain(a => audioIds.Contains(a.AudioId));
        }
        
        [Fact]
        public async Task ShouldNotAddAudios_WhenPlaylistDoesNotBelongToUser()
        {
            var (userId, _) = await _sliceFixture.RunAsUserAsync("kopacetic");
            var playlistFaker = new PlaylistFaker(userId);
            var playlist = playlistFaker.Generate();
            await _sliceFixture.InsertAsync(playlist);

            var (otherUserId, _) = await _sliceFixture.RunAsDefaultUserAsync();
            var audioFaker = new AudioFaker(otherUserId);
            var audios = audioFaker.Generate(3);
            var audioIds = audios.Select(a => a.Id).ToList();
            await _sliceFixture.InsertRangeAsync(audios);

            var request = new AddAudiosToPlaylistCommand(playlist.Id, audioIds);
            var result = await _sliceFixture.SendAsync(request);
            var loadedPlaylist = await _sliceFixture.ExecuteDbContextAsync(db =>
            {
                return db.Playlists
                    .Include(a => a.Audios)
                    .Where(p => p.Id == playlist.Id).SingleOrDefaultAsync();
            });

            result.IsSuccess.Should().BeFalse();
            result.ErrorCode.Should().Be(ResultError.Forbidden);
            loadedPlaylist.Audios.Count.Should().Be(0);
            loadedPlaylist.Audios.Should().NotContain(a => audioIds.Contains(a.AudioId));
        }
    }
}