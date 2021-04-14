using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Audiochan.Core.Enums;
using Audiochan.Core.Features.Audios.CreateAudio;
using Audiochan.Core.Features.Audios.GetAudio;
using Audiochan.Core.Helpers;
using Audiochan.Core.Models.ViewModels;
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
            var audio = new AudioBuilder(ownerId, "myaudio.mp3").Build();
            await _fixture.InsertAsync(audio);

            // Act
            var result = await _fixture.SendAsync(new GetAudioRequest(0));

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public async Task ShouldGetAudio_WhenAudioIsPrivateAndUserIsOwner()
        {
            // Assign
            var (adminId, _) = await _fixture.RunAsAdministratorAsync();
            var audio = new AudioBuilder(adminId, Guid.NewGuid() + ".mp3")
                .Publicity(Visibility.Private)
                .Build();
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
        public async Task ShouldGetAudio_WhenAudioIsPrivateAndPrivateKeyIsValid()
        {
            var (ownerId, _) = await _fixture.RunAsAdministratorAsync();
            var privateKey = "test";
            var audio = new AudioBuilder(ownerId)
                .Publicity(Visibility.Private, privateKey)
                .Build();
            await _fixture.InsertAsync(audio);

            await _fixture.RunAsDefaultUserAsync();
            var result = await _fixture.SendAsync(new GetAudioRequest(audio.Id, privateKey));

            result.Should().NotBeNull();
            result.Id.Should().Be(audio.Id);
        }
        
        [Fact]
        public async Task ShouldNotGetAudio_WhenAudioIsPrivateAndPrivateKeyIsInvalid()
        {
            var (ownerId, _) = await _fixture.RunAsAdministratorAsync();
            var privateKey = "test";
            var audio = new AudioBuilder(ownerId)
                .Publicity(Visibility.Private, privateKey)
                .Build();
            await _fixture.InsertAsync(audio);

            await _fixture.RunAsDefaultUserAsync();
            var result = await _fixture.SendAsync(new GetAudioRequest(audio.Id));

            result.Should().BeNull();
        }

        [Fact]
        public async Task ShouldGetAudio_WhenAudioIsUnlisted()
        {
            var (ownerId, _) = await _fixture.RunAsAdministratorAsync();
            var audio = new AudioBuilder(ownerId)
                .Publicity(Visibility.Unlisted)
                .Build();
            await _fixture.InsertAsync(audio);

            await _fixture.RunAsDefaultUserAsync();
            var result = await _fixture.SendAsync(new GetAudioRequest(audio.Id));

            result.Should().NotBeNull();
            result.Id.Should().Be(audio.Id);
        }

        [Fact]
        public async Task ShouldGetAudio()
        {
            // Assign
            await _fixture.RunAsDefaultUserAsync();

            var audio = await _fixture.SendAsync(new CreateAudioRequest
            {
                UploadId = UploadHelpers.GenerateUploadId(),
                FileName = "test.mp3",
                Duration = 100,
                FileSize = 100,
                Tags = new List<string> {"apples", "oranges"},
                Visibility = "unlisted"
            });

            // Act
            var result = await _fixture.SendAsync(new GetAudioRequest(audio.Data.Id));

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<AudioDetailViewModel>();
            result.Title.Should().Be(audio.Data.Title);
            result.Description.Should().Be(audio.Data.Description);
            result.Tags.Length.Should().Be(2);
            result.Tags.Should().Contain("apples");
            result.Tags.Should().Contain("oranges");
        }
    }
}