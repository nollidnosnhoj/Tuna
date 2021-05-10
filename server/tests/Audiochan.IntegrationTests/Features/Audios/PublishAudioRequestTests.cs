using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Audiochan.Core.Common.Builders;
using Audiochan.Core.Common.Enums;
using Audiochan.Core.Common.Extensions;
using Audiochan.Core.Features.Audios.PublishAudio;
using Audiochan.Core.Features.Audios.UploadAudio;
using Bogus;
using FluentAssertions;
using Xunit;

namespace Audiochan.IntegrationTests.Features.Audios
{
    [Collection(nameof(SliceFixture))]
    public class PublishAudioRequestTests
    {
        private readonly SliceFixture _fixture;
        private readonly Randomizer _randomizer;

        public PublishAudioRequestTests(SliceFixture fixture)
        {
            _fixture = fixture;
            _randomizer = new Randomizer();
        }

        [Fact]
        public async Task ShouldPublishSuccessfully()
        {
            // Assign
            await _fixture.RunAsAdministratorAsync();
            var uploadRequest = new UploadAudioRequest
            {
                Duration = _randomizer.Number(60, 300) + _randomizer.Decimal(),
                FileSize = _randomizer.Number(5_000, 5_000_000),
                FileName = string.Join('_', _randomizer.WordsArray(5)) + ".mp3"
            };
            var uploadAudioResponse = await _fixture.SendAsync(uploadRequest);

            // Act
            var currentTime = new DateTime(2021, 5, 5, 12, 0, 0);
            _fixture.SetCurrentTime(currentTime);
            var publishedAudioRequest = new PublishAudioRequest
            {
                AudioId = uploadAudioResponse.AudioId,
                Title = "Test Audio",
                Description = "This is a test description.",
                Tags = new List<string> {"test", "dotnet"},
                IsPublic = true
            };

            var publishedAudio = await _fixture.SendAsync(publishedAudioRequest);

            // Assert
            var audio = await _fixture.ExecuteDbContextAsync(dbContext =>
                dbContext.Audios.FindAsync(uploadAudioResponse.AudioId));
            
            publishedAudio.IsSuccess.Should().BeTrue();
            publishedAudio.Data.Should().NotBeNull();
            publishedAudio.Data.Title.Should().Be(publishedAudioRequest.Title);
            publishedAudio.Data.Description.Should().Be(publishedAudioRequest.Description);
            publishedAudio.Data.Tags.Should().Contain("test");
            publishedAudio.Data.Tags.Should().Contain("dotnet");
            publishedAudio.Data.IsPublic.Should().BeTrue();

            audio.Should().NotBeNull();
            audio.Duration.Should().Be(uploadRequest.Duration);
            audio.FileSize.Should().Be(uploadRequest.FileSize);
            audio.OriginalFileName.Should().Be(uploadRequest.FileName);
            audio.FileName.Should().Contain(audio.Id + audio.FileExt);
            audio.IsPublish.Should().BeTrue();
            audio.PublishDate.Should().Be(currentTime);
        }

        [Fact]
        public async Task ShouldNotPublish_WhenUserIsForbidden()
        {
            // Assign
            await _fixture.RunAsAdministratorAsync();
            var uploadRequest = new UploadAudioRequest
            {
                Duration = _randomizer.Number(60, 300) + _randomizer.Decimal(),
                FileSize = _randomizer.Number(5_000, 5_000_000),
                FileName = string.Join('_', _randomizer.WordsArray(5)) + ".mp3"
            };
            var uploadAudioResponse = await _fixture.SendAsync(uploadRequest);
            
            // Act
            await _fixture.RunAsDefaultUserAsync();
            var publishedAudioRequest = new PublishAudioRequest
            {
                AudioId = uploadAudioResponse.AudioId,
                Title = "Test Audio",
                Description = "This is a test description.",
                Tags = new List<string> {"test", "dotnet"},
                IsPublic = true
            };

            var publishedAudio = await _fixture.SendAsync(publishedAudioRequest);

            // Assert
            publishedAudio.IsSuccess.Should().BeFalse();
            publishedAudio.ErrorCode.Should().BeEquivalentTo(ResultError.Forbidden);
        }

        [Fact]
        public async Task ShouldNotPublish_WhenAudioIsAlreadyPublished()
        {
            // Assign
            var (ownerId, _) = await _fixture.RunAsAdministratorAsync();
            var audio = await new AudioBuilder()
                .AddUserId(ownerId)
                .AddFileName(_randomizer.Word() + ".mp3")
                .AddDuration(_randomizer.Number(60, 300))
                .AddFileSize(_randomizer.Number(5_000, 5_000_000))
                .AddTitle(_randomizer.Words().Truncate(30))
                .SetPublish(DateTime.UtcNow)
                .BuildAsync();

            await _fixture.InsertAsync(audio);
            
            // Act
            var publishedAudioRequest = new PublishAudioRequest
            {
                AudioId = audio.Id,
            };

            var publishedAudio = await _fixture.SendAsync(publishedAudioRequest);

            // Assert
            publishedAudio.IsSuccess.Should().BeFalse();
            publishedAudio.ErrorCode.Should().BeEquivalentTo(ResultError.BadRequest);
        }
    }
}