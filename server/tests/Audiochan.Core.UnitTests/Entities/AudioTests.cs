using System;
using System.Linq;
using Audiochan.Core.Common.Helpers;
using Audiochan.Core.Entities;
using FluentAssertions;
using Xunit;

namespace Audiochan.Core.UnitTests.Entities
{
    public class AudioTests
    {
        public readonly User ValidUser = new("testuser", "testuser@localhost", DateTime.UtcNow)
        {
            Id = "ValidUserId"
        };

        [Fact]
        public void NewAudio_ShouldThrow_WhenFileNameIsNullOrEmpty()
        {
            FluentActions.Invoking(() => new Audio(UploadHelpers.GenerateUploadId(), null, 0, 0, "ValidUserId"))
                .Should()
                .ThrowExactly<ArgumentNullException>("null", "fileName");

            FluentActions.Invoking(() => new Audio(UploadHelpers.GenerateUploadId(), string.Empty, 0, 0, "ValidUserId"))
                .Should()
                .ThrowExactly<ArgumentNullException>("empty", "fileName");

            FluentActions.Invoking(() => new Audio(UploadHelpers.GenerateUploadId(), "  ", 0, 0, "ValidUserId"))
                .Should()
                .ThrowExactly<ArgumentNullException>("whitespace", "fileName");
        }

        [Theory]
        [InlineData("audio.mp3")]
        public void NewAudio_ShouldNotThrow_WhenFilenameDoesHaveExtension(string fileName)
        {
            FluentActions.Invoking(() => new Audio(UploadHelpers.GenerateUploadId(), fileName, 0, 0, "ValidUserId"))
                .Should()
                .NotThrow<ArgumentException>();
        }

        [Fact]
        public void NewAudio_ShouldTruncate_WhenFilenameIsMoreThanThirtyCharactersLong()
        {
            const string fileName = "Vn4Emz1X9FJodmQxtKYszmnZBH6SM4o34MmVLXYKOjvizDK39l.mp3";
            var audio = new Audio(UploadHelpers.GenerateUploadId(), fileName, 100, 100, "ValidUserId");
            audio.Title.Length.Should().Be(30);
        }

        [Theory]
        [InlineData("shouldfail")]
        public void NewAudio_ShouldThrow_WhenFilenameDoesNotHaveExtension(string fileName)
        {
            FluentActions.Invoking(() => new Audio(UploadHelpers.GenerateUploadId(), fileName, 0, 0, "ValidUserId"))
                .Should()
                .Throw<ArgumentException>("no file extension", "filename");
        }

        [Fact]
        public void NewAudio_ShouldHaveExtension_BasedOnFileName()
        {
            var expectedTitle = "audio";
            var audio = new Audio(UploadHelpers.GenerateUploadId(), "audio.mp3", 0, 0, "ValidUserId");
            audio.Title.Should().Be(expectedTitle);
        }

        [Theory]
        [InlineData("apples", "oranges", "cucumber")]
        public void NewAudio_ShouldHaveCorrectTagValues(params string[] tags)
        {
            // Assign
            var tagEntities = tags.Select(tag => new Tag {Id = tag}).ToList();
            var audio = new Audio();

            // Act
            audio.UpdateTags(tagEntities);

            // Assert
            audio.Tags.Count.Should().Be(3);
            audio.Tags.Count(t => t.Id == "apples").Should().Be(1);
        }

        [Theory]
        [InlineData("apples", "oranges", "cucumber")]
        public void UpdateAudio_ShouldHaveCorrectTagValues(params string[] tags)
        {
            // Assign
            var tagEntities = tags.Select(tag => new Tag {Id = tag}).ToList();
            var audio = new Audio();
            audio.Tags.Add(new Tag {Id = "apples"});

            // Act
            audio.UpdateTags(tagEntities);

            // Assert
            audio.Tags.Count.Should().Be(3);
            audio.Tags.Count(t => t.Id == "apples").Should().Be(1);
        }
    }
}