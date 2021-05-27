using System;
using Audiochan.API.Features.Shared.Helpers;
using FluentAssertions;
using Xunit;

namespace Audiochan.Core.UnitTests.Helpers
{
    public class CursorHelperTests
    {
        [Fact]
        public void CursorEncodeDecodeTest()
        {
            var dateTime = new DateTime(2021, 12, 25, 12, 30, 30, 500);
            var id = "testId";

            var cursor = CursorHelpers.EncodeCursor(dateTime, id);

            var (assertDateTime, assertId) = CursorHelpers.DecodeCursor(cursor);

            assertDateTime.Should().NotBeNull();
            assertDateTime.Should().Be(dateTime);
            assertId.Should().Be(id);
        }
    }
}