using System.Collections.Generic;
using System.Threading.Tasks;
using Audiochan.Core.Users.GetUserFavoriteAudios;
using Audiochan.Domain.Entities;
using Audiochan.Tests.Common.Fakers.Audios;
using Bogus;
using FluentAssertions;
using Xunit;

namespace Audiochan.Core.IntegrationTests.Features.Users
{
    public class GetUserFavoriteAudiosTest : TestBase
    {
        private readonly Faker _faker;

        public GetUserFavoriteAudiosTest(TestFixture testFixture) : base(testFixture)
        {
            _faker = new Faker();
        }

        [Fact]
        public async Task ShouldReturnFavoriteAudioSuccessfully()
        {
            // Assign
            var (targetId, _) = await RunAsDefaultUserAsync();
            var audioFaker = new AudioFaker(targetId);
            var audios = audioFaker
                .Generate(3);
            InsertIntoDatabase(audios.ToArray());
            var (observerId, observerUsername) = await RunAsUserAsync();
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