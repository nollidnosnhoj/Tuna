using System;
using System.Threading.Tasks;
using Audiochan.Core.Common.Constants;
using Audiochan.Core.Common.Models;
using Audiochan.Core.Entities.Enums;
using Audiochan.Core.Features.Audios.GetAudio;
using Audiochan.Tests.Common.Fakers.Audios;
using FluentAssertions;
using Xunit;

namespace Audiochan.Core.IntegrationTests.Features.Audios
{
    [Collection(nameof(SliceFixture))]
    public class GetAudioTests
    {
        private readonly SliceFixture _fixture;

        public GetAudioTests(SliceFixture fixture)
        {
            _fixture = fixture;
        }
        
        [Fact]
        public async Task ShouldNotGetAudio_WhenAudioIdIsInvalid()
        {
            // Assign
            var (ownerId, _) = await _fixture.RunAsDefaultUserAsync();
            var audio = new AudioFaker(ownerId).Generate();
            
            await _fixture.InsertAsync(audio);

            // Act
            var result = await _fixture.SendAsync(new GetAudioQuery(Guid.Empty));

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public async Task ShouldGetAudio_WhenCreatorViews()
        {
            // Assign
            var (userId, _) = await _fixture.RunAsDefaultUserAsync();

            var audio = new AudioFaker(userId).Generate();
            
            await _fixture.InsertAsync(audio);

            // Act
            var result = await _fixture.SendAsync(new GetAudioQuery(audio.Id));

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<AudioDetailViewModel>();
        }
        
        [Fact]
        public async Task ShouldSuccessfullyGetAudio()
        {
            // Assign
            var (userId, _) = await _fixture.RunAsAdministratorAsync();

            var audio = new AudioFaker(userId).Generate();
            audio.Visibility = Visibility.Public;
            await _fixture.InsertAsync(audio);

            await _fixture.RunAsDefaultUserAsync();

            // Act
            var result = await _fixture.SendAsync(new GetAudioQuery(audio.Id));

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<AudioDetailViewModel>();
            result!.Title.Should().Be(audio.Title);
            result.Description.Should().Be(audio.Description);
            result.Created.Should().BeCloseTo(audio.Created);
            result.Duration.Should().Be(audio.Duration);
            result.Picture.Should().BeNullOrEmpty();
            result.Tags.Count.Should().Be(audio.Tags.Count);
            result.AudioUrl.Should().Be(string.Format(MediaLinkInvariants.AudioUrl, audio.File));
            result.Size.Should().Be(audio.Size);
            result.Visibility.Should().Be(audio.Visibility);
            result.LastModified.Should().BeNull();
            result.User.Should().NotBeNull();
            result.User.Should().BeOfType<MetaAuthorDto>();
            result.User.Id.Should().Be(userId);
        }

        [Fact]
        public async Task ShouldCacheSuccessfully()
        {
            // Assign
            var (userId, _) = await _fixture.RunAsAdministratorAsync();

            var audio = new AudioFaker(userId).Generate();
            await _fixture.InsertAsync(audio);
            var result = await _fixture.SendAsync(new GetAudioQuery(audio.Id));

            // Act
            var (cacheExists, cacheResult) = await _fixture.GetCache<AudioDetailViewModel>(CacheKeys.Audio.GetAudio(audio.Id));
            
            // Assert
            cacheExists.Should().BeTrue();
            cacheResult.Should().NotBeNull();
            cacheResult.Should().BeOfType<AudioDetailViewModel>();
            cacheResult!.Title.Should().Be(result!.Title);
            cacheResult.Description.Should().Be(result!.Description);
            cacheResult.Created.Should().BeCloseTo(result!.Created);
            cacheResult.Duration.Should().Be(result!.Duration);
            cacheResult.Picture.Should().BeNullOrEmpty();
            cacheResult.Tags.Count.Should().Be(result!.Tags.Count);
            cacheResult.Size.Should().Be(result!.Size);
            cacheResult.Visibility.Should().Be(result!.Visibility);
            result.LastModified.Should().BeNull();
            result.User.Should().NotBeNull();
            result.User.Should().BeOfType<MetaAuthorDto>();
            result.User.Id.Should().Be(userId);
        }
    }
}