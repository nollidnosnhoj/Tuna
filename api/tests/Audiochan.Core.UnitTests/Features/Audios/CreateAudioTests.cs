using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Audiochan.Core.Common.Interfaces;
using Audiochan.Core.Common.Settings;
using Audiochan.Core.Entities;
using Audiochan.Core.Features.Audios.CreateAudio;
using Audiochan.Core.Services;
using Audiochan.Tests.Common.Builders;
using Audiochan.Tests.Common.Fakers.Audios;
using Audiochan.Tests.Common.Mocks;
using FluentAssertions;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;

namespace Audiochan.Core.UnitTests.Features.Audios
{
    public class CreateAudioTests
    {
        private readonly Mock<IStorageService> _storageService;
        private readonly Mock<ICurrentUserService> _currentUserService;
        private readonly Mock<IUnitOfWork> _unitOfWork;
        private readonly Mock<ICacheService> _cacheService;
        private readonly CreateAudioCommandHandler _handler;
        
        public CreateAudioTests()
        {
            var options = Options.Create(new MediaStorageSettings
            {
                Audio = MediaStorageSettingBuilder.BuildAudioDefault()
            });
            _storageService = new Mock<IStorageService>();
            _currentUserService = new Mock<ICurrentUserService>();
            _unitOfWork = new UnitOfWorkMock().Create();
            _cacheService = new Mock<ICacheService>();
            _handler = new CreateAudioCommandHandler(options, 
                _storageService.Object, 
                _currentUserService.Object,
                _unitOfWork.Object, _cacheService.Object);
        }

        [Fact]
        public async Task CreateAudio()
        {
            // Assign
            var user = new User
            {
                Id = Guid.NewGuid().ToString(),
                UserName = "testusername"
            };
            var request = new CreateAudioRequestFaker().Generate();
            _currentUserService
                .Setup(x => x.GetUserId())
                .Returns(user.Id);
            _currentUserService
                .Setup(x => x.GetUsername())
                .Returns(user.UserName);
            _storageService.Setup(x => x.ExistsAsync(
                    It.IsAny<string>(), 
                    It.IsAny<string>(), 
                    It.IsAny<string>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);
            _storageService.Setup(x => x.MoveBlobAsync(
                    It.IsAny<string>(), 
                    It.IsAny<string>(), 
                    It.IsAny<string>(),
                    It.IsAny<string>(), 
                    It.IsAny<string>(), 
                    It.IsAny<string?>(), 
                    It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);
            _unitOfWork
                .Setup(x => x.Audios.AddAsync(
                    It.IsAny<Audio>(), 
                    It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);
            _unitOfWork
                .Setup(x => x.Tags.GetAppropriateTags(
                    It.IsAny<List<string>>(), 
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(request.Tags.Select(x => new Tag{Name=x}).ToList());
            _unitOfWork
                .Setup(x => x.Users.LoadAsync(
                    It.IsAny<Expression<Func<User, bool>>>(),
                    It.IsAny<bool>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(user);
            _cacheService
                .Setup(x => x.SetAsync(It.IsAny<string>(), It.IsAny<object>(), It.IsAny<TimeSpan?>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            // Act
            var response = await _handler.Handle(request, CancellationToken.None);
            
            // Assert
            response.Should().NotBeNull();
            response.IsSuccess.Should().BeTrue();
            response.Data.Should().NotBeNull();
            response.Data!.Title.Should().Be(request.Title);
            response.Data.Description.Should().Be(request.Description);
            response.Data.Visibility.Should().Be(request.Visibility);
            response.Data.Duration.Should().Be(request.Duration);
            response.Data.FileSize.Should().Be(request.FileSize);
            response.Data.Author.Should().NotBeNull();
            response.Data.Author.Id.Should().Be(user.Id);
            response.Data.Author.Username.Should().Be(user.UserName);
        }
    }
}