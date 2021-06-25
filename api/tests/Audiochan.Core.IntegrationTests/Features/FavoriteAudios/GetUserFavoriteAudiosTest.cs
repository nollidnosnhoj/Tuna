using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Audiochan.Core.Entities;
using Audiochan.Core.Features.Users.GetUserFavoriteAudios;
using Audiochan.Tests.Common.Fakers.Audios;
using Bogus;
using FluentAssertions;
using Xunit;

namespace Audiochan.Core.IntegrationTests.Features.FavoriteAudios
{
    [Collection(nameof(SliceFixture))]
    public class GetUserFavoriteAudiosTest
    {
        private readonly SliceFixture _sliceFixture;
        private readonly Faker _faker;

        public GetUserFavoriteAudiosTest(SliceFixture sliceFixture)
        {
            _sliceFixture = sliceFixture;
            _faker = new Faker();
        }

        [Fact]
        public async Task ShouldReturnFavoriteAudioSuccessfully()
        {
            // Assign
            var (targetId, _) = await _sliceFixture.RunAsAdministratorAsync();
            var audioFaker = new AudioFaker(targetId);
            var audios = audioFaker.Generate(3);
            await _sliceFixture.InsertAsync(audios.ToArray());
            var (observerId, observerUsername) = await _sliceFixture.RunAsUserAsync(
                _faker.Random.String2(15), 
                _faker.Internet.Password(), 
                Array.Empty<string>());
            var favoriteAudios = new List<FavoriteAudio>();
            foreach (var audio in audios)
            {
                var favoriteAudio = new FavoriteAudio
                {
                    AudioId = audio.Id,
                    UserId = observerId,
                    FavoriteDate = new DateTime(2020, 12, 25)
                };
                
                favoriteAudios.Add(favoriteAudio);
            }
            await _sliceFixture.InsertAsync(favoriteAudios.ToArray());

            // Act
            var response = await _sliceFixture.SendAsync(new GetUserFavoriteAudiosQuery
            {
                Username = observerUsername
            });

            // Assert
            response.Should().NotBeNull();
            response.Count.Should().Be(3);
        }
    }
}