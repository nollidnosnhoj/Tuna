using System.Threading.Tasks;
using Audiochan.Core.Users.CheckIfAudioFavorited;
using Audiochan.Domain.Entities;
using Audiochan.Tests.Common.Fakers.Audios;
using Bogus;
using FluentAssertions;
using Xunit;

namespace Audiochan.Core.IntegrationTests.Features.Users
{
    public class CheckIfUserFavoritedAudioTests : TestBase
    {
        private readonly Faker _faker;

        public CheckIfUserFavoritedAudioTests(TestFixture testFixture) : base(testFixture)
        {
            _faker = new Faker();
        }

        [Fact]
        public async Task ShouldReturnTrue_WhenUserFavoritedAudio()
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
            var isFavorited = await SendAsync(new CheckIfAudioFavoritedQuery(audio.Id, observerId));

            // Assert
            isFavorited.Should().BeTrue();
        }
        
        [Fact]
        public async Task ShouldReturnFalse_WhenUserDidNotFavorite()
        {
            // Assign
            var (targetId, _) = await RunAsDefaultUserAsync();
            var audio = new AudioFaker(targetId).Generate();
            InsertIntoDatabase(audio);
            var (observerId, _) = await RunAsUserAsync();

            // Act
            var isFavorited = await SendAsync(new CheckIfAudioFavoritedQuery(audio.Id, observerId));

            // Assert
            isFavorited.Should().BeFalse();
        }
    }
}