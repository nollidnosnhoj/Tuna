using System;
using Audiochan.Core.Common.Helpers;
using Audiochan.Core.Common.Interfaces;
using FluentAssertions;
using Microsoft.IdentityModel.Tokens;
using Moq;
using Xunit;

namespace Audiochan.UnitTests.Helpers
{
    public class CursorHelperTests
    {
        private readonly Mock<IDateTimeProvider> _dateTimeProviderMock = new();

        [Fact]
        public void CursorEncodeDecodeTest()
        {
            var dateTime = new DateTime(2021, 12, 25).ToUniversalTime();
            var id = "testId";

            _dateTimeProviderMock
                .Setup(x => x.FromEpochToDateTime(It.IsAny<long>()))
                .Returns<long>(EpochTime.DateTime);
            _dateTimeProviderMock
                .Setup(x => x.FromDateTimeToEpoch(It.IsAny<DateTime>()))
                .Returns<DateTime>(EpochTime.GetIntDate);

            var cursor = CursorHelpers.EncodeCursor(_dateTimeProviderMock.Object, dateTime, id);

            var (assertDateTime, assertId) = CursorHelpers.DecodeCursor(_dateTimeProviderMock.Object, cursor);

            assertDateTime.Should().Be(dateTime);
            assertId.Should().Be(id);
        }
    }
}