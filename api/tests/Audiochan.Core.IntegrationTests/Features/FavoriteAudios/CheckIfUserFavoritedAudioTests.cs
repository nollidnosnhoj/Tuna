using System;
using System.Threading.Tasks;
using Audiochan.Core.Entities;
using Audiochan.Core.Features.FavoriteAudios.CheckIfFavoriting;
using Audiochan.Tests.Common.Fakers.Audios;
using Bogus;
using FluentAssertions;
using Xunit;

namespace Audiochan.Core.IntegrationTests.Features.FavoriteAudios
{
    [Collection(nameof(SliceFixture))]
    public class CheckIfUserFavoritedAudioTests
    {
        private readonly SliceFixture _sliceFixture;
        private readonly Faker _faker;

        public CheckIfUserFavoritedAudioTests(SliceFixture sliceFixture)
        {
            _sliceFixture = sliceFixture;
            _faker = new Faker();
        }

        [Fact]
        public async Task ShouldReturnTrue_WhenUserFavoritedAudio()
        {
            var (targetId, _) = await _sliceFixture.RunAsAdministratorAsync();
            var audio = new AudioFaker(targetId).Generate();
            await _sliceFixture.InsertAsync(audio);
            var (observerId, _) = await _sliceFixture.RunAsUserAsync(
                _faker.Random.String2(15), 
                _faker.Internet.Password(), 
                Array.Empty<string>());
            var favoriteAudio = new FavoriteAudio
            {
                AudioId = audio.Id,
                UserId = observerId,
                FavoriteDate = DateTime.Now
            };
            await _sliceFixture.InsertAsync(favoriteAudio);

            var isFavorited = await _sliceFixture.SendAsync(new CheckIfUserFavoritedAudioQuery(audio.Id, observerId));

            isFavorited.Should().BeTrue();
        }
        
        [Fact]
        public async Task ShouldReturnFalse_WhenUserDidNotFavorite()
        {
            var (targetId, _) = await _sliceFixture.RunAsAdministratorAsync();
            var audio = new AudioFaker(targetId).Generate();
            await _sliceFixture.InsertAsync(audio);
            var (observerId, _) = await _sliceFixture.RunAsUserAsync(
                _faker.Random.String2(15), 
                _faker.Internet.Password(), 
                Array.Empty<string>());

            var isFavorited = await _sliceFixture.SendAsync(new CheckIfUserFavoritedAudioQuery(audio.Id, observerId));

            isFavorited.Should().BeFalse();
        }
        
        [Fact]
        public async Task ShouldReturnFalse_WhenUserUnfavorited()
        {
            var (targetId, _) = await _sliceFixture.RunAsAdministratorAsync();
            var audio = new AudioFaker(targetId).Generate();
            await _sliceFixture.InsertAsync(audio);
            var (observerId, _) = await _sliceFixture.RunAsUserAsync(
                _faker.Random.String2(15), 
                _faker.Internet.Password(), 
                Array.Empty<string>());
            var now = DateTime.UtcNow;
            var favoriteAudio = new FavoriteAudio
            {
                AudioId = audio.Id,
                UserId = observerId,
                FavoriteDate = now.AddDays(-1),
                UnfavoriteDate = now
            };
            await _sliceFixture.InsertAsync(favoriteAudio);

            var isFavorited = await _sliceFixture.SendAsync(new CheckIfUserFavoritedAudioQuery(audio.Id, observerId));

            isFavorited.Should().BeFalse();
        }
    }
}