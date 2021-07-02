using System.Threading;
using System.Threading.Tasks;
using Audiochan.Core.Common.Interfaces;
using Moq;

namespace Audiochan.Tests.Common.Mocks
{
    public class UnitOfWorkMock
    {
        private readonly Mock<IUnitOfWork> _mock;

        public UnitOfWorkMock()
        {
            _mock = new Mock<IUnitOfWork>();
            _mock.Setup(x => x.BeginTransaction());
            _mock.Setup(x => x.CommitTransactionAsync()).Returns(Task.CompletedTask);
            _mock.Setup(x => x.RollbackTransaction());
            _mock.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);
        }

        public Mock<IUnitOfWork> Create()
        {
            return _mock;
        }
    }
}