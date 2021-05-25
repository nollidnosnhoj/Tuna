using System;
using System.Threading.Tasks;
using Audiochan.Core.Common.Constants;
using Audiochan.Core.Features.Audios;
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
            var result = await _fixture.SendAsync(new GetAudioRequest(Guid.Empty));

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public async Task ShouldGetAudio()
        {
            // Assign
            var (userId, _) = await _fixture.RunAsDefaultUserAsync();

            var audio = new AudioFaker(userId).Generate();
            
            await _fixture.InsertAsync(audio);

            // Act
            var result = await _fixture.SendAsync(new GetAudioRequest(audio.Id));

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<AudioDetailViewModel>();
            result!.Title.Should().Be(audio.Title);
            result.Description.Should().Be(audio.Description);
            result.Created.Should().BeCloseTo(audio.Created);
            result.Duration.Should().Be(audio.Duration);
            result.Picture.Should().BeNullOrEmpty();
            result.Tags.Count.Should().Be(audio.Tags.Count);
            result.AudioUrl.Should().Be(string.Format(MediaLinkInvariants.AudioUrl, audio.Id, audio.FileName));
            result.FileExt.Should().Be(audio.FileExt);
            result.FileSize.Should().Be(audio.FileSize);
            result.IsPublic.Should().Be(audio.IsPublic);
            result.LastModified.Should().BeNull();
        }
    }
}