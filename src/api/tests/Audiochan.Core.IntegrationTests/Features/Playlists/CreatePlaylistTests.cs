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
    public class CreatePlaylistTests : TestBase
    {
        public CreatePlaylistTests(TestFixture testFixture) : base(testFixture)
        {
        }

        [Fact]
        public async Task ShouldCreateSuccessfully()
        {
            var (userId, _) = await RunAsDefaultUserAsync();
            var audios = new AudioFaker(userId)
                .WithVisibility(Visibility.Public)
                .Generate(3);
            InsertRange(audios);
            var audioIds = audios.Select(x => x.Id).ToList();

            var request = new CreatePlaylistCommandFaker(audioIds).Generate();
            var result = await SendAsync(request);
            var playlist = ExecuteDbContext(db =>
            {
                return db.Playlists
                    .Include(p => p.Audios)
                    .SingleOrDefault(p => p.Id == result.Data);
            });

            result.Should().NotBeNull();
            result.IsSuccess.Should().BeTrue();
            result.Data.Should().BeGreaterThan(0);
            playlist.Should().NotBeNull();
            playlist!.Title.Should().Be(request.Title);
            playlist.Description.Should().BeNullOrEmpty();
            playlist.Visibility.Should().Be(request.Visibility);
            playlist.Audios.Should().NotBeEmpty();
            playlist.Audios.Select(p => p.AudioId).Should().Contain(audioIds);
        }
        
        [Fact]
        public async Task ShouldCreatePrivateSuccessfully()
        {
            var (userId, _) = await RunAsDefaultUserAsync();
            var audios = new AudioFaker(userId)
                .WithVisibility(Visibility.Public)
                .Generate(3);
            InsertRange(audios);
            var audioIds = audios.Select(x => x.Id).ToList();
            var request = new CreatePlaylistCommandFaker(audioIds)
                .SetFixedVisibility(Visibility.Private)
                .Generate();
            var result = await SendAsync(request);
            var playlist = ExecuteDbContext(db =>
            {
                return db.Playlists
                    .SingleOrDefault(p => p.Id == result.Data);
            });

            result.Should().NotBeNull();
            result.IsSuccess.Should().BeTrue();
            result.Data.Should().BeGreaterThan(0);
            playlist.Should().NotBeNull();
            playlist!.Visibility.Should().Be(Visibility.Private);
            playlist!.Secret.Should().NotBeNullOrEmpty();
        }
    }
}