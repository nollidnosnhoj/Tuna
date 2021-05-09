using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Audiochan.Core.Common.Builders;
using Audiochan.Core.Common.Helpers;
using Audiochan.Core.Features.Audios;
using Audiochan.Core.Features.Audios.PublishAudio;
using Audiochan.Core.Features.Audios.GetAudio;
using Audiochan.UnitTests;
using FluentAssertions;
using NodaTime;
using Xunit;

namespace Audiochan.IntegrationTests.Features.Audios
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
            var audio = await new AudioBuilder()
                .UseTestDefaults(ownerId, "myaudio.mp3")
                .BuildAsync();
            await _fixture.InsertAsync(audio);

            // Act
            var result = await _fixture.SendAsync(new GetAudioRequest(string.Empty));

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public async Task ShouldGetAudio_WhenAudioIsPrivateAndUserIsOwner()
        {
            // Assign
            var (adminId, _) = await _fixture.RunAsAdministratorAsync();
            var audio = await new AudioBuilder()
                .UseTestDefaults(adminId, Guid.NewGuid() + ".mp3")
                .SetPublic(false)
                .BuildAsync();
            await _fixture.InsertAsync(audio);

            // Act
            var successResult = await _fixture.SendAsync(new GetAudioRequest(audio.Id));
            await _fixture.RunAsDefaultUserAsync();
            var failureResult = await _fixture.SendAsync(new GetAudioRequest(audio.Id));

            // Assert
            successResult.Should().NotBeNull();
            successResult.Should().BeOfType<AudioDetailViewModel>();
            failureResult.Should().BeNull();
        }

        [Fact]
        public async Task ShouldGetAudio()
        {
            // Assign
            var (userId, _) = await _fixture.RunAsDefaultUserAsync();

            var audio = await new AudioBuilder()
                .AddFileName("test.mp3")
                .AddTitle("Test Song")
                .AddFileSize(100)
                .AddDuration(100)
                .AddUserId(userId)
                .SetPublic(true)
                .SetPublish(true, Instant.FromDateTimeUtc(DateTime.UtcNow))
                .BuildAsync();
            
            await _fixture.InsertAsync(audio);

            // Act
            var result = await _fixture.SendAsync(new GetAudioRequest(audio.Id));

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<AudioDetailViewModel>();
            result.Title.Should().Be(audio.Title);
            result.Tags.Length.Should().Be(2);
            result.Tags.Should().Contain("apples");
            result.Tags.Should().Contain("oranges");
        }
    }
}