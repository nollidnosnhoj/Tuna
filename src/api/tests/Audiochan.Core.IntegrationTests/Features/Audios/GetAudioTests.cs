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
    public class GetAudioTests : TestBase
    {
        public GetAudioTests(TestFixture fixture) : base(fixture)
        {
        }
        
        [Fact]
        public async Task ShouldNotGetAudio_WhenAudioIdIsInvalid()
        {
            // Assign
            var (ownerId, _) = await RunAsDefaultUserAsync();
            var audio = new AudioFaker(ownerId).Generate();
            
            Insert(audio);

            // Act
            var result = await SendAsync(new GetAudioQuery(0));

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public async Task ShouldGetAudio_WhenCreatorViews()
        {
            // Assign
            var (userId, _) = await RunAsDefaultUserAsync();

            var audio = new AudioFaker(userId).Generate();
            
            Insert(audio);

            // Act
            var result = await SendAsync(new GetAudioQuery(audio.Id));

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<AudioViewModel>();
        }
        
        [Fact]
        public async Task ShouldSuccessfullyGetAudio()
        {
            // Assign
            var (userId, _) = await RunAsAdministratorAsync();

            var audio = new AudioFaker(userId).Generate();
            audio.Visibility = Visibility.Public;
            Insert(audio);

            await RunAsDefaultUserAsync();

            // Act
            var result = await SendAsync(new GetAudioQuery(audio.Id));

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<AudioViewModel>();
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
        public async Task ShouldSuccessfullyGetPrivateAudio_WhenSecretIsValid()
        {
            // Assign
            var (userId, _) = await RunAsAdministratorAsync();

            var audio = new AudioFaker(userId)
                .WithVisibility(Visibility.Private)
                .Generate();
            
            Insert(audio);

            await RunAsDefaultUserAsync();

            // Act
            var result = await SendAsync(new GetAudioQuery(audio.Id, audio.Secret));

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<AudioViewModel>();
        }

        [Fact]
        public async Task ShouldNotGetPrivateAudio()
        {
            // Assign
            var (userId, _) = await RunAsAdministratorAsync();

            var audio = new AudioFaker(userId)
                .WithVisibility(Visibility.Private)
                .Generate();
            
            Insert(audio);

            await RunAsDefaultUserAsync();

            // Act
            var result = await SendAsync(new GetAudioQuery(audio.Id));

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public async Task ShouldCacheSuccessfully()
        {
            // Assign
            var (userId, _) = await RunAsAdministratorAsync();

            var audio = new AudioFaker(userId).Generate();
            Insert(audio);
            var result = await SendAsync(new GetAudioQuery(audio.Id));

            // Act
            var (cacheExists, cacheResult) = await GetCache<AudioViewModel>(CacheKeys.Audio.GetAudio(audio.Id));
            
            // Assert
            cacheExists.Should().BeTrue();
            cacheResult.Should().NotBeNull();
            cacheResult.Should().BeOfType<AudioViewModel>();
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