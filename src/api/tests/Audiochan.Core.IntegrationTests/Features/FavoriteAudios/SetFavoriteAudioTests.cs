using System;
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
    [Collection(nameof(SliceFixture))]
    public class SetFavoriteAudioTests
    {
        private readonly SliceFixture _sliceFixture;
        private readonly Faker _faker;

        public SetFavoriteAudioTests(SliceFixture sliceFixture)
        {
            _sliceFixture = sliceFixture;
            _faker = new Faker();
        }
        
        [Fact]
        public async Task AddFavoriteTest()
        {
            // Assign
            var (targetId, _) = await _sliceFixture.RunAsDefaultUserAsync();
            var (observerId, _) = await _sliceFixture.RunAsUserAsync(
                    _faker.Random.String2(15), 
                    _faker.Internet.Password(), 
                    Array.Empty<string>());

            var audio = new AudioFaker(targetId).Generate();
            await _sliceFixture.InsertAsync(audio);

            // Act
            await _sliceFixture.SendAsync(new SetFavoriteAudioCommand(audio.Id, observerId, true));

            var refetchAudio = await _sliceFixture.ExecuteDbContextAsync(database =>
            {
                return database.Audios
                    .AsNoTracking()
                    .Include(u => u.Favorited)
                    .SingleOrDefaultAsync(a => a.Id == audio.Id);
            });

            // Assert
            refetchAudio.Favorited.Should().NotBeEmpty();
            refetchAudio.Favorited.Should().Contain(x => x.UserId == observerId && x.AudioId == audio.Id);
        }

        [Fact]
        public async Task ShouldSuccessfullyUnfavoriteAudio()
        {
            // Assign
            var (targetId, _) = await _sliceFixture.RunAsDefaultUserAsync();
            var (observerId, _) = await _sliceFixture.RunAsUserAsync(
                _faker.Random.String2(15), 
                _faker.Internet.Password(), 
                Array.Empty<string>());
            
            var audio = new AudioFaker(targetId).Generate();
            await _sliceFixture.InsertAsync(audio);

            var favoriteAudio = new FavoriteAudio
            {
                AudioId = audio.Id,
                UserId = observerId,
                FavoriteDate = DateTime.UtcNow
            };
            await _sliceFixture.InsertAsync(favoriteAudio);

            // Act
            await _sliceFixture.SendAsync(new SetFavoriteAudioCommand(audio.Id, observerId, false));

            var refetchAudio = await _sliceFixture.ExecuteDbContextAsync(database =>
            {
                return database.Audios
                    .AsNoTracking()
                    .Include(u => u.Favorited)
                    .SingleOrDefaultAsync(a => a.Id == audio.Id);
            });

            // Assert
            refetchAudio.Favorited.Should().BeEmpty();
        }
    }
}