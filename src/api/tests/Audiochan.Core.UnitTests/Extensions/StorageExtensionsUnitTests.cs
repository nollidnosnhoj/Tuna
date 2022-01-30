using System;
using System.IO;
using Audiochan.Core.Commons.Extensions;
using FluentAssertions;
using Xunit;

namespace Audiochan.Core.UnitTests.Extensions
{
    public class StorageExtensionsUnitTests
    {
        private readonly Guid _uploadId = Guid.NewGuid();

        [Fact]
        public void GetValidContentTypeTest()
        {
            var contentType = "source.mp3".GetContentType();
            contentType.Should().Be("audio/mpeg");
        }

        [Fact]
        public void GetInvalidContentTypeTest()
        {
            var contentType = "source".GetContentType();
            contentType.Should().Be("application/octet-stream");
        }

        [Fact]
        public void GetValidContentTypeFromPathTest()
        {
            var path = Path.Combine("audios", _uploadId.ToString(), "source.mp3");
            var contentType = path.GetContentType();
            contentType.Should().Be("audio/mpeg");
        }
    }
}