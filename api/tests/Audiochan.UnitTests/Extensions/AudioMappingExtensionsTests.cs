using Audiochan.Core.Common.Extensions.MappingExtensions;
using Audiochan.Core.Common.Settings;
using Audiochan.Core.Entities;
using Audiochan.UnitTests.Builders;
using FluentAssertions;
using Xunit;

namespace Audiochan.UnitTests.Extensions
{
    public class AudioMappingExtensionsTests
    {
        private readonly MediaStorageSettings _storageSettings = new()
        {
            Audio = MediaStorageSettingBuilder.BuildAudioDefault(),
            Image = MediaStorageSettingBuilder.BuildImageDefault()
        };

        [Fact]
        public void ShouldNotThrow_WhenMapping()
        {
            var user = new User {Id = "testId", UserName = "testuser"};
            var audio = new AudioBuilder()
                .UseTestDefaults(user.Id)
                .AddUser(user)
                .Build("uploadId");
            FluentActions.Invoking(() => audio.MapToDetail(_storageSettings)).Should().NotThrow();
        }
        
        [Fact]
        public void SuccessfullyMapAudioToDetail()
        {
            var user = new User {Id = "testId", UserName = "testuser"};
            var audio = new AudioBuilder()
                .UseTestDefaults(user.Id)
                .AddUser(user)
                .Build("uploadId");
            var model = audio.MapToDetail(_storageSettings, true);
            model.Should().NotBeNull();
            model.Id.Should().Be(audio.Id);
            model.Title.Should().Be(audio.Title);
            model.Description.Should().Be(audio.Description);
            model.Duration.Should().Be(audio.Duration);
            model.FileExt.Should().Be(audio.FileExt);
            model.FileSize.Should().Be(audio.FileSize);
            model.Created.Should().Be(audio.Created);
            model.IsPublic.Should().Be(audio.IsPublic);
            model.Picture.Should().BeNullOrEmpty();
            model.Author.Id.Should().Be(audio.User.Id);
            model.Author.Username.Should().Be(audio.User.UserName);
            model.Author.Picture.Should().BeNullOrEmpty();
        }
    }
}