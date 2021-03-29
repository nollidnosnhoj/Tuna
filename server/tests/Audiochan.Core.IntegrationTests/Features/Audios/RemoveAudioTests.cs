using System;
using System.Threading.Tasks;
using Audiochan.Core.Common.Models.Responses;
using Audiochan.Core.Entities;
using Audiochan.Core.Features.Audios.RemoveAudio;
using Audiochan.Core.UnitTests.Builders;
using FluentAssertions;
using Xunit;

namespace Audiochan.Core.IntegrationTests.Features.Audios
{
    [Collection(nameof(SliceFixture))]
    public class RemoveAudioTests
    {
        private readonly SliceFixture _fixture;

        public RemoveAudioTests(SliceFixture fixture)
        {
            _fixture = fixture;
        }

        [Fact]
        public async Task ShouldNotRemoveAudio_WhenUserCannotModify()
        {
            // Assign
            var (ownerId, _) =
                await _fixture.RunAsUserAsync("kopacetic", Guid.NewGuid().ToString(), Array.Empty<string>());

            var audio = new AudioBuilder(ownerId, "testaudio.mp3").Build();

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
            var audio = new AudioBuilder(ownerId, "testaudio.mp3").Build();
            await _fixture.InsertAsync(audio);

            var command = new RemoveAudioRequest(audio.Id);
            var result = await _fixture.SendAsync(command);

            var created = await _fixture.FindAsync<Audio, long>(audio.Id);

            result.Should().NotBeNull();
            result.IsSuccess.Should().Be(true);

            created.Should().BeNull();
        }
    }
}