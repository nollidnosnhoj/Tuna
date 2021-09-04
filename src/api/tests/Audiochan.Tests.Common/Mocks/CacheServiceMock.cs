using System.Threading;
using Audiochan.Core.Common.Interfaces;
using Audiochan.Core.Common.Interfaces.Services;
using Moq;

namespace Audiochan.Tests.Common.Mocks
{
    public class CacheServiceMock
    {
        private readonly Mock<ICacheService> _mock;

        public CacheServiceMock()
        {
            _mock = new Mock<ICacheService>();
            _mock.Setup(x => x.RemoveAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);
            _mock.Setup(x => x.RemoveAsync(It.IsAny<ICacheOptions>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);
        }

        public Mock<ICacheService> Create()
        {
            return _mock;
        }
    }
}