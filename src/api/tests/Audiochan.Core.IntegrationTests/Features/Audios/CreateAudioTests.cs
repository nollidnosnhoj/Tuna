﻿using System.Threading.Tasks;
using Audiochan.Core.Audios;
using Audiochan.Core.Audios.GetAudio;
using Audiochan.Core.Common;
using Audiochan.Tests.Common.Fakers.Audios;
using FluentAssertions;
using NUnit.Framework;

namespace Audiochan.Core.IntegrationTests.Features.Audios
{
    using static TestFixture;
    
    public class CreateAudioTests : TestBase
    {
        [Test]
        public async Task SuccessfullyCreateAudio()
        {
            // Assign
            var (userId, userName) = await RunAsDefaultUserAsync();
            var request = new CreateAudioRequestFaker().Generate();

            // Act
            var response = await SendAsync(request);
            var audio = await SendAsync(new GetAudioQuery(response.Data));

            // Assert
            response.IsSuccess.Should().BeTrue();
            response.Data.Should().BeGreaterThan(0);
            audio.Should().NotBeNull();
            audio!.Title.Should().Be(request.Title);
            audio.Description.Should().Be(request.Description);
            audio.Duration.Should().Be(request.Duration);
            audio.Size.Should().Be(request.FileSize);
            audio.User.Should().NotBeNull();
            audio.User.Id.Should().Be(userId);
            audio.User.UserName.Should().Be(userName);
        }
        
        [Test]
        public async Task ShouldCreateCacheSuccessfully()
        {
            // Assign
            await RunAsDefaultUserAsync();
            var request = new CreateAudioRequestFaker()
                .Generate();
            var response = await SendAsync(request);
            await SendAsync(new GetAudioQuery(response.Data)); // trigger the caching
            
            // Act
            var audio = await GetCache<AudioDto>(CacheKeys.Audio.GetAudio(response.Data));

            // Assert
            audio.Should().NotBeNull();
            audio.Should().BeOfType<AudioDto>();
        }
    }
}