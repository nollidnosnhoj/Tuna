using System.Threading.Tasks;
using Audiochan.Core.Common.Constants;
using Audiochan.Core.Features.Playlists.GetPlaylistDetail;
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
            var playlist = new PlaylistFaker(userId).Generate();
            await _sliceFixture.InsertAsync(playlist);

            var response = await _sliceFixture
                .SendAsync(new GetPlaylistDetailQuery(playlist.Id));

            response.Should().NotBeNull();
            response.Should().BeOfType<PlaylistDetailViewModel>();
            response!.Title.Should().Be(playlist.Title);
            response.Description.Should().Be(playlist.Description);
            response.Visibility.Should().Be(playlist.Visibility);
            response.Picture.Should().BeNullOrEmpty();
        }
    }
}