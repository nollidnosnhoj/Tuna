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
    public class GetAudioRequestTests
    {
        private readonly SliceFixture _fixture;

        public GetAudioRequestTests(SliceFixture fixture)
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
            audio.UpdateVisibility(Visibility.Private);
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
            audio.UpdateVisibility(Visibility.Private);
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
            audio.UpdateVisibility(Visibility.Public);
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
    }
}