using System;
using Audiochan.Core.Common.Helpers;
using FluentAssertions;
using Xunit;

namespace Audiochan.UnitTests.Helpers
{
    public class CursorHelperTests
    {
        [Fact]
        public void CursorEncodeDecodeTest()
        {
            var dateTime = new DateTime(2021, 12, 25, 12, 30, 30, 500).ToUniversalTime();
            var id = "testId";

            var cursor = CursorHelpers.EncodeCursor(dateTime, id);

            var (assertDateTime, assertId) = CursorHelpers.DecodeCursor(cursor);

            assertDateTime.Should().NotBeNull();
            assertDateTime.Should().Be(dateTime);
            assertDateTime!.Value.Millisecond.Should().Be(dateTime.Millisecond);
            assertId.Should().Be(id);
        }
    }
}