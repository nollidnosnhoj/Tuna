using System.Linq;
using System.Threading.Tasks;
using Audiochan.Core.Common.Models;
using Audiochan.Core.Entities;
using Audiochan.Core.Entities.Enums;
using Audiochan.Core.Features.Playlists.RemoveAudiosFromPlaylist;
using Audiochan.Tests.Common.Fakers.Audios;
using Audiochan.Tests.Common.Fakers.Playlists;
using FluentAssertions;
using Xunit;

namespace Audiochan.Core.IntegrationTests.Features.Playlists
{
    public class RemoveAudiosFromPlaylistTests : TestBase
    {
        public RemoveAudiosFromPlaylistTests(TestFixture testFixture) : base(testFixture)
        {
        }

        [Fact]
        public async Task ShouldSuccessfullyRemoveAudiosFromPlaylist()
        {
            var (userId, _) = await RunAsDefaultUserAsync();
            
            var playlist = new PlaylistFaker(userId).Generate();
            InsertIntoDatabase(playlist);
            
            var audios = new AudioFaker(userId)
                .WithVisibility(Visibility.Public)
                .Generate(5);
            InsertRangeIntoDatabase(audios);
            
            var playlistAudios = audios
                .Select(a => new PlaylistAudio
                {
                    AudioId = a.Id,
                    PlaylistId = playlist.Id,
                }).ToList();
            InsertRangeIntoDatabase(playlistAudios);

            var playlistAudioIds = playlistAudios.Select(pa => pa.Id).ToList();
            var request = new RemoveAudiosFromPlaylistCommand(playlist.Id, playlistAudioIds);
            var result = await SendAsync(request);

            var newAudios = ExecuteDbContext(db =>
            {
                return db.PlaylistAudios
                    .Where(pa => pa.PlaylistId == playlist.Id)
                    .ToList();
            });

            result.IsSuccess.Should().BeTrue();
            newAudios.Should().BeEmpty();
        }

        [Fact]
        public async Task ShouldFail_WhenPlaylistDoesNotBelongToUser()
        {
            var (userId, _) = await RunAsDefaultUserAsync();
            var playlist = new PlaylistFaker(userId).Generate();
            InsertIntoDatabase(playlist);
            var audios = new AudioFaker(userId)
                .WithVisibility(Visibility.Public)
                .Generate(5);
            InsertRangeIntoDatabase(audios);
            var playlistAudios = audios
                .Select(a => new PlaylistAudio
                {
                    AudioId = a.Id,
                    PlaylistId = playlist.Id,
                }).ToList();
            InsertRangeIntoDatabase(playlistAudios);

            await RunAsUserAsync("testuser");
            var result = await SendAsync(new RemoveAudiosFromPlaylistCommand(
                playlist.Id, 
                playlistAudios.Select(pa => pa.Id).ToList()));

            result.IsSuccess.Should().BeFalse();
            result.ErrorCode.Should().Be(ResultError.Forbidden);
        }
    }
}