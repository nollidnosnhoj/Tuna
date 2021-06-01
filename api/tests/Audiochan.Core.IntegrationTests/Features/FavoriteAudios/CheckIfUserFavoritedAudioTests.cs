using System;
using System.Threading.Tasks;
using Audiochan.API.Features.FavoriteAudios.CheckIfFavoriting;
using Audiochan.Tests.Common.Fakers.Audios;
using FluentAssertions;
using Xunit;

namespace Audiochan.Core.IntegrationTests.Features.FavoriteAudios
{
    [Collection(nameof(SliceFixture))]
    public class CheckIfUserFavoritedAudioTests
    {
        private readonly SliceFixture _sliceFixture;

        public CheckIfUserFavoritedAudioTests(SliceFixture sliceFixture)
        {
            _sliceFixture = sliceFixture;
        }

        [Fact]
        public async Task ShouldReturnTrue_WhenUserFavoritedAudio()
        {
            var (targetId, _) = await _sliceFixture.RunAsAdministratorAsync();
            var audio = new AudioFaker(targetId).Generate();
            await _sliceFixture.InsertAsync(audio);
            var (observerId, _) = await _sliceFixture.RunAsDefaultUserAsync();
            var favoriteAudio = new Entities.FavoriteAudio
            {
                AudioId = audio.Id,
                UserId = observerId,
                FavoriteDate = DateTime.Now
            };
            await _sliceFixture.InsertAsync(favoriteAudio);

            var isFavorited = await _sliceFixture.SendAsync(new CheckIfUserFavoritedAudioRequest(audio.Id, observerId));

            isFavorited.Should().BeTrue();
        }
        
        [Fact]
        public async Task ShouldReturnFalse_WhenUserDidNotFavorite()
        {
            var (targetId, _) = await _sliceFixture.RunAsAdministratorAsync();
            var audio = new AudioFaker(targetId).Generate();
            await _sliceFixture.InsertAsync(audio);
            var (observerId, _) = await _sliceFixture.RunAsDefaultUserAsync();

            var isFavorited = await _sliceFixture.SendAsync(new CheckIfUserFavoritedAudioRequest(audio.Id, observerId));

            isFavorited.Should().BeFalse();
        }
        
        [Fact]
        public async Task ShouldReturnFalse_WhenUserUnfavorited()
        {
            var (targetId, _) = await _sliceFixture.RunAsAdministratorAsync();
            var audio = new AudioFaker(targetId).Generate();
            await _sliceFixture.InsertAsync(audio);
            var (observerId, _) = await _sliceFixture.RunAsDefaultUserAsync();
            var now = DateTime.UtcNow;
            var favoriteAudio = new Entities.FavoriteAudio
            {
                AudioId = audio.Id,
                UserId = observerId,
                FavoriteDate = now.AddDays(-1),
                UnfavoriteDate = now
            };
            await _sliceFixture.InsertAsync(favoriteAudio);

            var isFavorited = await _sliceFixture.SendAsync(new CheckIfUserFavoritedAudioRequest(audio.Id, observerId));

            isFavorited.Should().BeFalse();
        }
    }
}