using System.Linq;
using System.Threading.Tasks;
using Audiochan.Core.Entities.Enums;
using Audiochan.Tests.Common.Fakers.Audios;
using Audiochan.Tests.Common.Fakers.Playlists;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Audiochan.Core.IntegrationTests.Features.Playlists
{
    [Collection(nameof(SliceFixture))]
    public class CreatePlaylistTests
    {
        private readonly SliceFixture _sliceFixture;

        public CreatePlaylistTests(SliceFixture sliceFixture)
        {
            _sliceFixture = sliceFixture;
        }

        [Fact]
        public async Task ShouldCreateSuccessfully()
        {
            var (userId, _) = await _sliceFixture.RunAsDefaultUserAsync();
            var audios = new AudioFaker(userId)
                .SetFixedVisibility(Visibility.Public)
                .Generate(3);
            await _sliceFixture.InsertRangeAsync(audios);
            var audioIds = audios.Select(x => x.Id).ToList();

            var request = new CreatePlaylistCommandFaker(audioIds).Generate();
            var result = await _sliceFixture.SendAsync(request);
            var playlist = await _sliceFixture.ExecuteDbContextAsync(db =>
            {
                return db.Playlists
                    .Include(p => p.Audios)
                    .SingleOrDefaultAsync(p => p.Id == result.Data);
            });

            result.Should().NotBeNull();
            result.IsSuccess.Should().BeTrue();
            result.Data.Should().NotBeEmpty();
            playlist.Should().NotBeNull();
            playlist.Title.Should().Be(request.Title);
            playlist.Description.Should().BeNullOrEmpty();
            playlist.Visibility.Should().Be(request.Visibility);
            playlist.Audios.Should().NotBeEmpty();
            playlist.Audios.Select(p => p.AudioId).Should().Contain(audioIds);
        }
    }
}