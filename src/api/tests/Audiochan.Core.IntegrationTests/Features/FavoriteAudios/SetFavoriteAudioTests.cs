using System;
using System.Linq;
using System.Threading.Tasks;
using Audiochan.Core.Entities;
using Audiochan.Core.Features.FavoriteAudios.SetFavoriteAudio;
using Audiochan.Tests.Common.Fakers.Audios;
using Bogus;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Audiochan.Core.IntegrationTests.Features.FavoriteAudios
{
    public class SetFavoriteAudioTests : TestBase
    {
        private readonly Faker _faker;

        public SetFavoriteAudioTests(TestFixture testFixture) : base(testFixture)
        {
            _faker = new Faker();
        }
        
        [Fact]
        public async Task AddFavoriteTest()
        {
            // Assign
            var (targetId, _) = await RunAsDefaultUserAsync();
            var (observerId, _) = await RunAsUserAsync(_faker.Random.String2(15));

            var audio = new AudioFaker(targetId).Generate();
            Insert(audio);

            // Act
            await SendAsync(new SetFavoriteAudioCommand(audio.Id, observerId, true));

            var refetchAudio = ExecuteDbContext(database =>
            {
                return database.Audios
                    .AsNoTracking()
                    .Include(u => u.Favorited)
                    .SingleOrDefault(a => a.Id == audio.Id);
            });

            // Assert
            refetchAudio.Should().NotBeNull();
            refetchAudio!.Favorited.Should().NotBeEmpty();
            refetchAudio.Favorited.Should().Contain(x => x.UserId == observerId && x.AudioId == audio.Id);
        }

        [Fact]
        public async Task ShouldSuccessfullyUnfavoriteAudio()
        {
            // Assign
            var (targetId, _) = await RunAsDefaultUserAsync();
            var (observerId, _) = await RunAsUserAsync(_faker.Random.String2(15));
            
            var audio = new AudioFaker(targetId).Generate();
            Insert(audio);

            var favoriteAudio = new FavoriteAudio
            {
                AudioId = audio.Id,
                UserId = observerId,
            };
            Insert(favoriteAudio);

            // Act
            await SendAsync(new SetFavoriteAudioCommand(audio.Id, observerId, false));

            var refetchAudio = ExecuteDbContext(database =>
            {
                return database.Audios
                    .AsNoTracking()
                    .Include(u => u.Favorited)
                    .SingleOrDefault(a => a.Id == audio.Id);
            });

            // Assert
            refetchAudio.Should().NotBeNull();
            refetchAudio!.Favorited.Should().BeEmpty();
        }
    }
}