﻿using System;
using System.Threading.Tasks;
using Audiochan.Core.Entities;
using Audiochan.Core.Features.FavoriteAudios.CheckIfAudioFavorited;
using Audiochan.Tests.Common.Fakers.Audios;
using Bogus;
using FluentAssertions;
using Xunit;

namespace Audiochan.Core.IntegrationTests.Features.FavoriteAudios
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
            var (targetId, _) = await RunAsAdministratorAsync();
            var audio = new AudioFaker(targetId).Generate();
            Insert(audio);
            var (observerId, _) = await RunAsUserAsync(
                _faker.Random.String2(15), 
                _faker.Internet.Password(), 
                Array.Empty<string>());
            var favoriteAudio = new FavoriteAudio
            {
                AudioId = audio.Id,
                UserId = observerId,
                FavoriteDate = DateTime.Now
            };
            Insert(favoriteAudio);

            // Act
            var isFavorited = await SendAsync(new CheckIfAudioFavoritedQuery(audio.Id, observerId));

            // Assert
            isFavorited.Should().BeTrue();
        }
        
        [Fact]
        public async Task ShouldReturnFalse_WhenUserDidNotFavorite()
        {
            // Assign
            var (targetId, _) = await RunAsAdministratorAsync();
            var audio = new AudioFaker(targetId).Generate();
            Insert(audio);
            var (observerId, _) = await RunAsUserAsync(
                _faker.Random.String2(15), 
                _faker.Internet.Password(), 
                Array.Empty<string>());

            // Act
            var isFavorited = await SendAsync(new CheckIfAudioFavoritedQuery(audio.Id, observerId));

            // Assert
            isFavorited.Should().BeFalse();
        }
        
        [Fact]
        public async Task ShouldReturnFalse_WhenUserUnfavorited()
        {
            // Assign
            var (targetId, _) = await RunAsAdministratorAsync();
            var audio = new AudioFaker(targetId).Generate();
            Insert(audio);
            var (observerId, _) = await RunAsUserAsync(
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
            Insert(favoriteAudio);

            // Act
            var isFavorited = await SendAsync(new CheckIfAudioFavoritedQuery(audio.Id, observerId));

            // Assert
            isFavorited.Should().BeFalse();
        }
    }
}