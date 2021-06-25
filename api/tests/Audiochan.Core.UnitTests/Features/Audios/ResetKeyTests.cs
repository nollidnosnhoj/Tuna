using System;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Audiochan.Core.Common.Interfaces;
using Audiochan.Core.Entities;
using Audiochan.Core.Entities.Enums;
using Audiochan.Core.Features.Audios.ResetPrivateKey;
using Audiochan.Core.Services;
using Audiochan.Tests.Common.Fakers.Audios;
using Audiochan.Tests.Common.Mocks;
using FluentAssertions;
using Moq;
using Xunit;

namespace Audiochan.Core.UnitTests.Features.Audios
{
    public class ResetKeyTests
    {
        private readonly Mock<ICurrentUserService> _currentUserService;
        private readonly Mock<IUnitOfWork> _unitOfWork;
        private readonly ResetPrivateKeyCommandHandler _handler;

        public ResetKeyTests()
        {
            _currentUserService = new Mock<ICurrentUserService>();
            _unitOfWork = new UnitOfWorkMock().Create();
            _handler = new ResetPrivateKeyCommandHandler(_currentUserService.Object, _unitOfWork.Object);
        }
        
        [Fact]
        public async Task ShouldResetPrivateKey()
        {
            var userId = Guid.NewGuid().ToString();
            var audio = new AudioFaker(userId, true).Generate();
            audio.UpdateVisibility(Visibility.Private);
            var ogKey = audio.PrivateKey;
            _currentUserService.Setup(x => x.GetUserId()).Returns(userId);
            _unitOfWork.Setup(x => x.Audios.FindAsync(
                    It.IsAny<Expression<Func<Audio, bool>>>(), 
                    It.IsAny<bool>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(audio);
            var result = await _handler.Handle(new ResetPrivateKeyCommand(audio.Id), CancellationToken.None);

            audio.PrivateKey.Should().NotBeNullOrEmpty();
            result.IsSuccess.Should().BeTrue();
            result.Data.Should().NotBeNullOrEmpty();
            result.Data.Should().NotBe(ogKey);
        }
    }
}