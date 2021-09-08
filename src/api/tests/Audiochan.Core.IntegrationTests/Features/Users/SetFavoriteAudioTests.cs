using System.Linq;
using System.Threading.Tasks;
using Audiochan.Core.Users.SetFavoriteAudio;
using Audiochan.Domain.Entities;
using Audiochan.Tests.Common.Fakers.Audios;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;

namespace Audiochan.Core.IntegrationTests.Features.Users
{
    using static TestFixture;

    public class SetFavoriteAudioTests : TestBase
    {
        [Test]
        public async Task AddFavoriteTest()
        {
            // Assign
            var (targetId, _) = await RunAsDefaultUserAsync();
            var (observerId, _) = await RunAsUserAsync();

            var audio = new AudioFaker(targetId).Generate();
            InsertIntoDatabase(audio);

            // Act
            await SendAsync(new SetFavoriteAudioCommand(audio.Id, observerId, true));

            var refetchAudio = ExecuteDbContext(database =>
            {
                return database.Audios
                    .AsNoTracking()
                    .Include(u => u.FavoriteAudios)
                    .SingleOrDefault(a => a.Id == audio.Id);
            });

            // Assert
            refetchAudio.Should().NotBeNull();
            refetchAudio!.FavoriteAudios.Should().NotBeEmpty();
            refetchAudio.FavoriteAudios.Should().Contain(x => x.UserId == observerId);
        }

        [Test]
        public async Task ShouldSuccessfullyUnfavoriteAudio()
        {
            // Assign
            var (targetId, _) = await RunAsDefaultUserAsync();
            
            var audio = new AudioFaker(targetId).Generate();
            InsertIntoDatabase(audio);
            
            var (observerId, _) = await RunAsUserAsync();

            var favoriteAudio = new FavoriteAudio
            {
                AudioId = audio.Id,
                UserId = observerId,
            };
            InsertIntoDatabase(favoriteAudio);

            // Act
            await SendAsync(new SetFavoriteAudioCommand(audio.Id, observerId, false));

            var refetchAudio = ExecuteDbContext(database =>
            {
                return database.Audios
                    .AsNoTracking()
                    .Include(u => u.FavoriteAudios)
                    .SingleOrDefault(a => a.Id == audio.Id);
            });

            // Assert
            refetchAudio.Should().NotBeNull();
            refetchAudio!.FavoriteAudios.Should().BeEmpty();
        }
    }
}