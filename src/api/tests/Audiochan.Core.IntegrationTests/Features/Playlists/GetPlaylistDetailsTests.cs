using System;
using System.Linq;
using System.Threading.Tasks;
using Audiochan.Core.Playlists;
using Audiochan.Core.Playlists.GetPlaylist;
using Audiochan.Domain.Entities;
using Audiochan.Tests.Common.Fakers.Audios;
using Audiochan.Tests.Common.Fakers.Playlists;
using FluentAssertions;
using NUnit.Framework;

namespace Audiochan.Core.IntegrationTests.Features.Playlists
{
    using static TestFixture;

    public class GetPlaylistDetailsTests : TestBase
    {
        [Test]
        public async Task ShouldSuccessfullyFetchPlaylist()
        {
            var (userId, _) = await RunAsDefaultUserAsync();
            var audioFaker = new AudioFaker(userId);
            var audios = audioFaker.Generate(5);
            var playlist = new PlaylistFaker(userId).Generate();
            InsertRangeIntoDatabase(audios);
            InsertIntoDatabase(playlist);
            var now = DateTime.UtcNow;
            var playlistAudios = audios
                .Select((t, i) => new PlaylistAudio
                {
                    PlaylistId = playlist.Id,
                    AudioId = t.Id,
                }).ToList();
            InsertRangeIntoDatabase(playlistAudios);

            var response = await SendAsync(new GetPlaylistQuery(playlist.Id));

            response.Should().NotBeNull();
            response.Should().BeOfType<PlaylistDto>();
            response!.Title.Should().Be(playlist.Title);
            response.Description.Should().Be(playlist.Description);
            response.Picture.Should().BeNullOrEmpty();
        }
    }
}