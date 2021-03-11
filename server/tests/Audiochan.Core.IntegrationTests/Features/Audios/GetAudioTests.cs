using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Audiochan.Core.Common.Helpers;
using Audiochan.Core.Common.Models.Responses;
using Audiochan.Core.Features.Audios.CreateAudio;
using Audiochan.Core.Features.Audios.GetAudio;
using Audiochan.Core.UnitTests.Builders;
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
            var audio = new AudioBuilder("myaudio.mp3", ownerId).Build();
            await _fixture.InsertAsync(audio);

            // Act
            var result = await _fixture.SendAsync(new GetAudioQuery(0));

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().Be(false);
            result.ErrorCode.Should().Be(ResultError.NotFound);
        }

        [Fact]
        public async Task ShouldGetAudio_WhenAudioIsPrivateAndUserIsOwner()
        {
            // Assign
            var (adminId, _) = await _fixture.RunAsAdministratorAsync();
            var audio = new AudioBuilder(Guid.NewGuid() + ".mp3", adminId)
                .Public(false)
                .Build();
            await _fixture.InsertAsync(audio);

            // Act
            var successResult = await _fixture.SendAsync(new GetAudioQuery(audio.Id));
            await _fixture.RunAsDefaultUserAsync();
            var failureResult = await _fixture.SendAsync(new GetAudioQuery(audio.Id));

            // Assert
            successResult.Should().NotBeNull();
            successResult.IsSuccess.Should().Be(true);
            successResult.Data.Should().NotBeNull();
            successResult.Data.Should().BeOfType<AudioDetailViewModel>();
            failureResult.Should().NotBeNull();
            failureResult.IsSuccess.Should().Be(false);
            failureResult.ErrorCode.Should().Be(ResultError.NotFound);
        }

        [Fact]
        public async Task ShouldGetAudio()
        {
            // Assign
            await _fixture.RunAsDefaultUserAsync();

            var audio = await _fixture.SendAsync(new CreateAudioCommand
            {
                UploadId = UploadHelpers.GenerateUploadId(),
                FileName = "test.mp3",
                Duration = 100,
                FileSize = 100,
                Genre = "dubstep",
                Tags = new List<string> {"apples", "oranges"}
            });

            // Act
            var result = await _fixture.SendAsync(new GetAudioQuery(audio.Data.Id));

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().Be(true);
            result.Data.Should().NotBeNull();
            result.Data.Should().BeOfType<AudioDetailViewModel>();
            result.Data.Should().NotBeNull();
            result.Data.Title.Should().Be(audio.Data.Title);
            result.Data.Description.Should().Be(audio.Data.Description);
            result.Data.Genre.Should().NotBeNull();
            result.Data.Genre.Slug.Should().Be("dubstep");
            result.Data.Tags.Length.Should().Be(2);
            result.Data.Tags.Should().Contain("apples");
            result.Data.Tags.Should().Contain("oranges");
        }
    }
}