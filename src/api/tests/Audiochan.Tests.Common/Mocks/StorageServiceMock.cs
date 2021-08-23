using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Audiochan.Core.Interfaces;
using Moq;

namespace Audiochan.Tests.Common.Mocks
{
    public static class StorageServiceMock
    {
        public static Mock<IStorageService> Create()
        {
            var mock = new Mock<IStorageService>();
            mock
                .Setup(x =>
                    x.ExistsAsync(It.IsAny<string>(),
                        It.IsAny<string>(),
                        It.IsAny<string>(),
                        It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);
            mock
                .Setup(x =>
                    x.ExistsAsync(It.IsAny<string>(),
                        It.IsAny<string>(),
                        It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);
            mock
                .Setup(x =>
                    x.RemoveAsync(It.IsAny<string>(), 
                        It.IsAny<string>(),
                        It.IsAny<string>(),
                        It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);
            mock
                .Setup(x =>
                    x.RemoveAsync(It.IsAny<string>(),
                        It.IsAny<string>(),
                        It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);
            mock
                .Setup(x =>
                    x.SaveAsync(It.IsAny<Stream>(),
                        It.IsAny<string>(),
                        It.IsAny<string>(),
                        It.IsAny<string>(),
                        It.IsAny<Dictionary<string, string>>(),
                        It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);
            mock
                .Setup(x =>
                    x.SaveAsync(It.IsAny<Stream>(),
                        It.IsAny<string>(),
                        It.IsAny<string>(),
                        It.IsAny<Dictionary<string, string>>(),
                        It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask); 
            return mock;
        }
    }
}