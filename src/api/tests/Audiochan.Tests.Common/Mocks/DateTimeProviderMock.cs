using System;
using Audiochan.Application.Services;
using Moq;

namespace Audiochan.Tests.Common.Mocks
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