using System.Threading.Tasks;
using Audiochan.Application.Commons.Extensions;
using Audiochan.Application.Features.Audios.Models;
using Audiochan.Application.Features.Audios.Queries.GetAudio;
using Audiochan.Application.Features.Users.Models;
using Audiochan.Tests.Common.Fakers.Audios;
using FluentAssertions;
using NUnit.Framework;

namespace Audiochan.Application.IntegrationTests.Features.Audios
{
    using static TestFixture;

    public class GetAudioTests : TestBase
    {
        [Test]
        public async Task ShouldNotGetAudio_WhenAudioIdIsInvalid()
        {
            // Assign
            var owner = await RunAsDefaultUserAsync();
            owner.TryGetUserId(out var ownerId);
            var audio = new AudioFaker(ownerId).Generate();
            
            InsertIntoDatabase(audio);

            // Act
            var result = await SendAsync(new GetAudioQuery(0));

            // Assert
            result.Should().BeNull();
        }
        
        [Test]
        public async Task ShouldSuccessfullyGetAudio()
        {
            // Assign
            var user = await RunAsDefaultUserAsync();
            user.TryGetUserId(out var userId);

            var audio = new AudioFaker(userId).Generate();
            InsertIntoDatabase(audio);

            await RunAsDefaultUserAsync();

            // Act
            var result = await SendAsync(new GetAudioQuery(audio.Id));

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<AudioDto>();
            result!.Title.Should().Be(audio.Title);
            result.Description.Should().Be(audio.Description);
            result.Created.Should().Be(audio.Created);
            result.Duration.Should().Be(audio.Duration);
            result.Picture.Should().BeNullOrEmpty();
            result.Tags.Length.Should().Be(audio.Tags.Length);
            result.File.Should().Be(audio.File);
            result.Size.Should().Be(audio.Size);
            result.User.Should().NotBeNull();
            result.User.Should().BeOfType<UserDto>();
            result.User.Id.Should().Be(userId);
        }

        [Test]
        public async Task ShouldCacheSuccessfully()
        {
            // Assign
            var user = await RunAsDefaultUserAsync();
            user.TryGetUserId(out var userId);

            var audio = new AudioFaker(userId).Generate();
            InsertIntoDatabase(audio);
            var result = await SendAsync(new GetAudioQuery(audio.Id));

            // Act
            var cacheResult = await GetCache<AudioDto>(CacheKeys.Audio.GetAudio(audio.Id));
            
            // Assert
            cacheResult.Should().NotBeNull();
            cacheResult.Should().BeOfType<AudioDto>();
            cacheResult!.Title.Should().Be(result!.Title);
            cacheResult.Description.Should().Be(result!.Description);
            cacheResult.Created.Should().Be(result!.Created);
            cacheResult.Duration.Should().Be(result!.Duration);
            cacheResult.Picture.Should().BeNullOrEmpty();
            cacheResult.Tags.Length.Should().Be(result!.Tags.Length);
            cacheResult.Size.Should().Be(result!.Size);
            result.User.Should().NotBeNull();
            result.User.Should().BeOfType<UserDto>();
            result.User.Id.Should().Be(userId);
        }
    }
}