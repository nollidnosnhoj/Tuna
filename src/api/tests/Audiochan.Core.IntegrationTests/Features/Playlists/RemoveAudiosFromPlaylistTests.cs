using System;
using System.Linq;
using System.Threading.Tasks;
using Audiochan.Core.Common.Models;
using Audiochan.Core.Entities;
using Audiochan.Core.Entities.Enums;
using Audiochan.Core.Features.Playlists.RemoveAudiosFromPlaylist;
using Audiochan.Tests.Common.Fakers.Audios;
using Audiochan.Tests.Common.Fakers.Playlists;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Audiochan.Core.IntegrationTests.Features.Playlists
{
    [Collection(nameof(SliceFixture))]
    public class RemoveAudiosFromPlaylistTests
    {
        private readonly SliceFixture _sliceFixture;

        public RemoveAudiosFromPlaylistTests(SliceFixture sliceFixture)
        {
            _sliceFixture = sliceFixture;
        }

        [Fact]
        public async Task ShouldSuccessfullyRemoveAudiosFromPlaylist()
        {
            var (userId, _) = await _sliceFixture.RunAsDefaultUserAsync();
            var playlist = new PlaylistFaker(userId).Generate();
            await _sliceFixture.InsertAsync(playlist);
            var audios = new AudioFaker(userId)
                .SetFixedVisibility(Visibility.Public)
                .Generate(5);
            await _sliceFixture.InsertRangeAsync(audios);
            var playlistAudios = audios
                .Select(a => new PlaylistAudio
                {
                    AudioId = a.Id,
                    PlaylistId = playlist.Id,
                    Added = DateTime.UtcNow
                }).ToList();
            await _sliceFixture.InsertRangeAsync(playlistAudios);

            var result = await _sliceFixture.SendAsync(new RemoveAudiosFromPlaylistCommand(
                playlist.Id, 
                audios.Select(a => a.Id).ToList()));

            var newAudios = await _sliceFixture.ExecuteDbContextAsync(db =>
            {
                return db.PlaylistAudios.Where(pa => pa.PlaylistId == playlist.Id).ToListAsync();
            });

            result.IsSuccess.Should().BeTrue();
            newAudios.Should().BeEmpty();
        }

        [Fact]
        public async Task ShouldFail_WhenPlaylistDoesNotBelongToUser()
        {
            var (userId, _) = await _sliceFixture.RunAsDefaultUserAsync();
            var playlist = new PlaylistFaker(userId).Generate();
            await _sliceFixture.InsertAsync(playlist);
            var audios = new AudioFaker(userId)
                .SetFixedVisibility(Visibility.Public)
                .Generate(5);
            await _sliceFixture.InsertRangeAsync(audios);
            var audioIds = audios.Select(a => a.Id).ToList();
            var playlistAudios = audioIds
                .Select(a => new PlaylistAudio
                {
                    AudioId = a,
                    PlaylistId = playlist.Id,
                    Added = DateTime.UtcNow
                }).ToList();
            await _sliceFixture.InsertRangeAsync(playlistAudios);

            await _sliceFixture.RunAsUserAsync("testuser");
            var result = await _sliceFixture.SendAsync(new RemoveAudiosFromPlaylistCommand(playlist.Id, audioIds));

            result.IsSuccess.Should().BeFalse();
            result.ErrorCode.Should().Be(ResultError.Forbidden);
        }
    }
}