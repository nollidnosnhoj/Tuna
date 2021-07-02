using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Amazon.Runtime.Internal.Util;
using Audiochan.Core.Common.Interfaces;
using Audiochan.Core.Common.Models;
using Audiochan.Core.Entities;
using Audiochan.Core.Features.Audios;
using Audiochan.Core.Features.Audios.GetAudio;
using Audiochan.Core.Features.Audios.UpdateAudio;
using Audiochan.Core.Features.Users;
using Audiochan.Core.Services;
using Audiochan.Tests.Common.Fakers.Audios;
using Audiochan.Tests.Common.Mocks;
using AutoMapper;
using FluentAssertions;
using Moq;
using Xunit;

namespace Audiochan.Core.UnitTests.Features.Audios
{
    public class UpdateAudioTests
    {
        private readonly Mock<ICurrentUserService> _currentUserService;
        private readonly Mock<IUnitOfWork> _unitOfWork;
        private readonly UpdateAudioCommandHandler _handler;

        public UpdateAudioTests()
        {
            _currentUserService = new Mock<ICurrentUserService>();
            _unitOfWork = new UnitOfWorkMock().Create();
            _unitOfWork.Setup(x => x.Audios.Update(It.IsAny<Audio>()));
            var cacheService = new CacheServiceMock().Create();
            var mapperConfig = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<AudioMappingProfile>();
                cfg.AddProfile<UserMappingProfile>();
            });
            var mapper = mapperConfig.CreateMapper();
            _handler = new UpdateAudioCommandHandler(_currentUserService.Object, 
                _unitOfWork.Object, 
                cacheService.Object,
                mapper);
        }

        [Fact]
        public async Task ShouldNotUpdate_WhenUserCannotModify()
        {
            // Assign
            var user = new User {Id = Guid.NewGuid().ToString(), UserName = "testUser"};
            var audio = new AudioFaker(user.Id, true).Generate();
            audio.Tags.Clear();
            var command = new UpdateAudioRequestFaker(audio.Id).Generate();
            _currentUserService.Setup(x => x.GetUserId())
                .Returns("abc");
            _unitOfWork.Setup(x => x.Audios.LoadForUpdate(
                    It.IsAny<Guid>(), 
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(audio);
            if (command.Tags is not null)
            {
                _unitOfWork.Setup(x => x.Tags.GetAppropriateTags(
                        It.IsAny<List<string>>(),
                        It.IsAny<CancellationToken>()))
                    .ReturnsAsync(command.Tags.Select(x => new Tag{Name=x}).ToList());
            }
            // Act
            var result = await _handler.Handle(command, CancellationToken.None);
            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().Be(false);
            result.ErrorCode.Should().Be(ResultError.Forbidden);
        }

        [Fact]
        public async Task ShouldUpdateSuccessfully()
        {
            // Assign
            var user = new User {Id = Guid.NewGuid().ToString(), UserName = "testUser"};
            
            var audio = new AudioFaker(user.Id, true).Generate();
            audio.User = user;
            
            var command = new UpdateAudioRequestFaker(audio.Id).Generate();
            
            _currentUserService.Setup(x => x.GetUserId())
                .Returns(user.Id);
            _unitOfWork.Setup(x => x.Audios.LoadForUpdate(
                    It.IsAny<Guid>(), 
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(audio);
            if (command.Tags is not null)
            {
                _unitOfWork.Setup(x => x.Tags.GetAppropriateTags(
                        It.IsAny<List<string>>(),
                        It.IsAny<CancellationToken>()))
                    .ReturnsAsync(command.Tags.Select(x => new Tag{Name=x}).ToList());
            }
            
            // Act
            var result = await _handler.Handle(command, CancellationToken.None);
            
            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().Be(true);
            result.Data.Should().NotBeNull();
            result.Data.Should().BeOfType<AudioDetailViewModel>();
            result.Data.Should().NotBeNull();
            result.Data!.Title.Should().Be(command.Title);
            result.Data.Description.Should().Be(command.Description);
            result.Data.Tags.Count.Should().Be(command.Tags!.Count);
        }
    }
}