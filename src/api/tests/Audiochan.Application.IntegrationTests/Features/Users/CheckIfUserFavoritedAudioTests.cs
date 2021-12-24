using System.Threading.Tasks;
using Audiochan.Application.Commons.Extensions;
using Audiochan.Application.Features.Users.Queries.CheckIfAudioFavorited;
using Audiochan.Domain.Entities;
using Audiochan.Tests.Common.Fakers.Audios;
using FluentAssertions;
using NUnit.Framework;

namespace Audiochan.Application.IntegrationTests.Features.Users
{
    using static TestFixture;

    public class CheckIfUserFavoritedAudioTests : TestBase
    {
        [Test]
        public async Task ShouldReturnTrue_WhenUserFavoritedAudio()
        {
            // Assign
            var target = await RunAsDefaultUserAsync();
            target.TryGetUserId(out var targetId);
            
            var audio = new AudioFaker(targetId).Generate();
            
            InsertIntoDatabase(audio);
            
            var observer = await RunAsUserAsync();
            observer.TryGetUserId(out var observerId);
            
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
        
        [Test]
        public async Task ShouldReturnFalse_WhenUserDidNotFavorite()
        {
            // Assign
            var target = await RunAsDefaultUserAsync();
            target.TryGetUserId(out var targetId);
            
            var audio = new AudioFaker(targetId).Generate();
            
            InsertIntoDatabase(audio);
            
            var observer = await RunAsUserAsync();
            observer.TryGetUserId(out var observerId);

            // Act
            var isFavorited = await SendAsync(new CheckIfAudioFavoritedQuery(audio.Id, observerId));

            // Assert
            isFavorited.Should().BeFalse();
        }
    }
}