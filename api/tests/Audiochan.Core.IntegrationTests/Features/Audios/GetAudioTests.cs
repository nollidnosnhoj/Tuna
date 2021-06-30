using System.Threading.Tasks;
using Audiochan.Core.Common.Constants;
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
            var result = await _fixture.SendAsync(new GetAudioQuery(0));

            // Assert
            result.Should().BeNull();
        }
        
        [Fact]
        public async Task ShouldNotGetAudio_WhenPrivateKeyIsInvalid()
        {
            // Assign
            var (ownerId, _) = await _fixture.RunAsAdministratorAsync();
            var audio = new AudioFaker(ownerId).Generate();
            audio.Visibility = Visibility.Private;
            await _fixture.InsertAsync(audio);
            await _fixture.RunAsDefaultUserAsync();

            // Act
            var result = await _fixture.SendAsync(new GetAudioQuery(audio.Id));

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public async Task ShouldGetAudio_WhenPrivateKeyIsValid()
        {
            // Assign
            var (userId, _) = await _fixture.RunAsAdministratorAsync();

            var audio = new AudioFaker(userId).Generate();
            audio.Visibility = Visibility.Private;
            await _fixture.InsertAsync(audio);

            await _fixture.RunAsDefaultUserAsync();

            // Act
            var result = await _fixture.SendAsync(new GetAudioQuery(audio.Id, audio.PrivateKey));

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<AudioDetailViewModel>();
            result!.Title.Should().Be(audio.Title);
            result.Description.Should().Be(audio.Description);
            result.Created.Should().BeCloseTo(audio.Created);
            result.Duration.Should().Be(audio.Duration);
            result.Picture.Should().BeNullOrEmpty();
            result.Tags.Count.Should().Be(audio.Tags.Count);
            result.AudioUrl.Should().Be(string.Format(MediaLinkInvariants.AudioUrl, audio.Id, audio.BlobName));
            result.FileSize.Should().Be(audio.FileSize);
            result.Visibility.Should().Be(audio.Visibility);
            result.LastModified.Should().BeNull();
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
            result!.Title.Should().Be(audio.Title);
            result.Description.Should().Be(audio.Description);
            result.Created.Should().BeCloseTo(audio.Created);
            result.Duration.Should().Be(audio.Duration);
            result.Picture.Should().BeNullOrEmpty();
            result.Tags.Count.Should().Be(audio.Tags.Count);
            result.AudioUrl.Should().Be(string.Format(MediaLinkInvariants.AudioUrl, audio.Id, audio.BlobName));
            result.FileSize.Should().Be(audio.FileSize);
            result.Visibility.Should().Be(audio.Visibility);
            result.LastModified.Should().BeNull();
        }
        
        [Fact]
        public async Task ShouldGetAudio_WhenPublic()
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
            result.AudioUrl.Should().Be(string.Format(MediaLinkInvariants.AudioUrl, audio.Id, audio.BlobName));
            result.FileSize.Should().Be(audio.FileSize);
            result.Visibility.Should().Be(audio.Visibility);
            result.LastModified.Should().BeNull();
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
            cacheResult.FileSize.Should().Be(result!.FileSize);
            cacheResult.Visibility.Should().Be(result!.Visibility);
        }
    }
}