using System;
using System.Threading.Tasks;
using Audiochan.Core.Common.Builders;
using Audiochan.Core.Common.Enums;
using Audiochan.Core.Entities;
using Audiochan.Core.Features.Audios.RemoveAudio;
using Audiochan.UnitTests;
using FluentAssertions;
using Xunit;

namespace Audiochan.IntegrationTests.Features.Audios
{
    [Collection(nameof(SliceFixture))]
    public class RemoveAudioRequestTests
    {
        private readonly SliceFixture _fixture;

        public RemoveAudioRequestTests(SliceFixture fixture)
        {
            _fixture = fixture;
        }

        [Fact]
        public async Task ShouldNotRemoveAudio_WhenUserCannotModify()
        {
            // Assign
            var (ownerId, _) =
                await _fixture.RunAsUserAsync("kopacetic", Guid.NewGuid().ToString(), Array.Empty<string>());

            var audio = await new AudioBuilder()
                .UseTestDefaults(ownerId, true, "testaudio.mp3")
                .BuildAsync();

            await _fixture.InsertAsync(audio);

            // Act
            await _fixture.RunAsDefaultUserAsync();

            var command = new RemoveAudioRequest(audio.Id);

            var result = await _fixture.SendAsync(command);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().Be(false);
            result.ErrorCode.Should().Be(ResultError.Forbidden);
        }

        [Fact]
        public async Task ShouldRemoveAudio()
        {
            var (ownerId, _) = await _fixture.RunAsDefaultUserAsync();
            var audio = await new AudioBuilder()
                .UseTestDefaults(ownerId, true, "testaudio.mp3")
                .BuildAsync();
            await _fixture.InsertAsync(audio);

            var command = new RemoveAudioRequest(audio.Id);
            var result = await _fixture.SendAsync(command);

            var created = await _fixture.FindAsync<Audio, string>(audio.Id);

            result.Should().NotBeNull();
            result.IsSuccess.Should().Be(true);

            created.Should().BeNull();
        }
    }
}