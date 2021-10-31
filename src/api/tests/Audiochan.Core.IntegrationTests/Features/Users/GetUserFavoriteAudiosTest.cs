using System.Collections.Generic;
using System.Threading.Tasks;
using Audiochan.Core.Common.Extensions;
using Audiochan.Core.Users;
using Audiochan.Domain.Entities;
using Audiochan.Tests.Common.Fakers.Audios;
using FluentAssertions;
using NUnit.Framework;

namespace Audiochan.Core.IntegrationTests.Features.Users
{
    using static TestFixture;

    public class GetUserFavoriteAudiosTest : TestBase
    {
        [Test]
        public async Task ShouldReturnFavoriteAudioSuccessfully()
        {
            // Assign
            var target = await RunAsDefaultUserAsync();
            target.TryGetUserId(out var targetId);    
            
            var audioFaker = new AudioFaker(targetId);
            var audios = audioFaker
                .Generate(3);
            
            InsertIntoDatabase(audios.ToArray());
            
            var observer = await RunAsUserAsync();
            observer.TryGetUserId(out var observerId);
            observer.TryGetUserName(out var observerUsername);
            
            var favoriteAudios = new List<FavoriteAudio>();
            foreach (var audio in audios)
            {
                var favoriteAudio = new FavoriteAudio
                {
                    AudioId = audio.Id,
                    UserId = observerId,
                };
                
                favoriteAudios.Add(favoriteAudio);
            }
            InsertIntoDatabase(favoriteAudios.ToArray());

            // Act
            var response = await SendAsync(new GetUserFavoriteAudiosQuery
            {
                Username = observerUsername
            });

            // Assert
            response.Should().NotBeNull();
            response.Items.Count.Should().Be(3);
        }
    }
}