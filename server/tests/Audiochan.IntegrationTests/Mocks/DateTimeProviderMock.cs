using System;
using Audiochan.Core.Common.Interfaces;
using Moq;

namespace Audiochan.IntegrationTests.Mocks
{
    public static class DateTimeProviderMock
    {
        public static Mock<IDateTimeProvider> Create(DateTime dateTime)
        {
            var mock = new Mock<IDateTimeProvider>();
            mock.Setup(x => x.Now)
                .Returns(dateTime);
            return mock;
        }
    }
}