using System.Linq;
using System.Threading.Tasks;
using Audiochan.Tests.Common.Fakers.Audios;
using Audiochan.Tests.Common.Fakers.Playlists;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;

namespace Audiochan.Core.IntegrationTests.Features.Playlists
{
    using static TestFixture;

    public class CreatePlaylistTests : TestBase
    {
        [Test]
        public async Task ShouldCreateSuccessfully()
        {
            var (userId, _) = await RunAsDefaultUserAsync();
            var audios = new AudioFaker(userId)
                .Generate(3);
            InsertRangeIntoDatabase(audios);
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
            playlist.Audios.Should().NotBeEmpty();
            playlist.Audios.Select(p => p.Id).Should().Contain(audioIds);
        }
        
        [Test]
        public async Task ShouldCreatePrivateSuccessfully()
        {
            var (userId, _) = await RunAsDefaultUserAsync();
            var audios = new AudioFaker(userId)
                .Generate(3);
            InsertRangeIntoDatabase(audios);
            var audioIds = audios.Select(x => x.Id).ToList();
            var request = new CreatePlaylistCommandFaker(audioIds)
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
        }
    }
}