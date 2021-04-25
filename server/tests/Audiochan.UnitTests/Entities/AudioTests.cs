using System;
using System.Linq;
using Audiochan.Core.Entities;
using Audiochan.UnitTests.Mocks;
using FluentAssertions;
using Moq;
using Xunit;

namespace Audiochan.UnitTests.Entities
{
    public class AudioTests
    {
        private readonly User _validUser;
        private readonly IUploadHelperMock _uploadHelper;

        public AudioTests()
        {
            _validUser = new User("testuser", "testuser@localhost", DateTime.UtcNow)
            {
                Id = "ValidUserId"
            };
            
            var uploadHelperMock = new Mock<IUploadHelperMock>();
            uploadHelperMock.Setup(x => x.GenerateUploadId())
                .Returns(Guid.Empty.ToString());
            _uploadHelper = uploadHelperMock.Object;
        }

        [Fact]
        public void NewAudio_ShouldThrow_WhenFileNameIsNullOrEmpty()
        {
            FluentActions.Invoking(() => new Audio(
                    "Test", 
                    _uploadHelper.GenerateUploadId(), 
                    null, 
                    0, 
                    0, 
                    "ValidUserId"))
                .Should()
                .ThrowExactly<ArgumentNullException>("null", "fileName");

            FluentActions.Invoking(() => new Audio(
                    "Test", 
                    _uploadHelper.GenerateUploadId(), 
                    string.Empty, 
                    0, 
                    0, 
                    "ValidUserId"))
                .Should()
                .ThrowExactly<ArgumentNullException>("empty", "fileName");

            FluentActions.Invoking(() => new Audio(
                    "Test", 
                    _uploadHelper.GenerateUploadId(), 
                    "  ", 
                    0, 
                    0, 
                    "ValidUserId"))
                .Should()
                .ThrowExactly<ArgumentNullException>("whitespace", "fileName");
        }

        [Theory]
        [InlineData("audio.mp3")]
        public void NewAudio_ShouldNotThrow_WhenFilenameDoesHaveExtension(string fileName)
        {
            FluentActions.Invoking(() => new Audio(
                    "Test", 
                    _uploadHelper.GenerateUploadId(), 
                    fileName, 
                    0, 
                    0, 
                    "ValidUserId"))
                .Should()
                .NotThrow<ArgumentException>();
        }

        [Fact]
        public void NewAudio_ShouldTruncate_WhenFilenameIsMoreThanThirtyCharactersLong()
        {
            const string fileName = "Vn4Emz1X9FJodmQxtKYszmnZBH6SM4o34MmVLXYKOjvizDK39l.mp3";
            var audio = new Audio(
                "n4Emz1X9FJodmQxtKYszmnZBH6SM4o34MmVLXYKOjvizDK39l", 
                _uploadHelper.GenerateUploadId(), 
                fileName, 
                100, 
                100, 
                "ValidUserId");
            audio.Title.Length.Should().Be(30);
        }

        [Theory]
        [InlineData("shouldfail")]
        public void NewAudio_ShouldThrow_WhenFilenameDoesNotHaveExtension(string fileName)
        {
            FluentActions.Invoking(() => new Audio(
                    "Test", 
                    _uploadHelper.GenerateUploadId(), 
                    fileName, 
                    0, 
                    0, 
                    "ValidUserId"))
                .Should()
                .Throw<ArgumentException>("no file extension", "filename");
        }

        [Fact]
        public void NewAudio_ShouldHaveExtension_BasedOnFileName()
        {
            var ext = ".mp3";
            var audio = new Audio(
                "Audio", 
                _uploadHelper.GenerateUploadId(), 
                "audio.mp3", 
                0, 
                0, 
                "ValidUserId");
            audio.FileExt.Should().BeEquivalentTo(ext);
        }

        [Theory]
        [InlineData("apples", "oranges", "cucumber")]
        public void NewAudio_ShouldHaveCorrectTagValues(params string[] tags)
        {
            // Assign
            var tagEntities = tags.Select(tag => new Tag {Name = tag}).ToList();
            var audio = new Audio();

            // Act
            audio.UpdateTags(tagEntities);

            // Assert
            audio.Tags.Count.Should().Be(3);
            audio.Tags.Count(t => t.Name == "apples").Should().Be(1);
        }

        [Theory]
        [InlineData("apples", "oranges", "cucumber")]
        public void UpdateAudio_ShouldHaveCorrectTagValues(params string[] tags)
        {
            // Assign
            var tagEntities = tags.Select(tag => new Tag {Name = tag}).ToList();
            var audio = new Audio();

            // Act
            audio.UpdateTags(tagEntities);

            // Assert
            audio.Tags.Count.Should().Be(3);
            audio.Tags.Count(t => t.Name == "apples").Should().Be(1);
        }
    }
}