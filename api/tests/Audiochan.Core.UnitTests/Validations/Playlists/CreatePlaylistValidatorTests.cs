using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using Audiochan.Core.Common.Interfaces;
using Audiochan.Core.Entities;
using Audiochan.Core.Features.Playlists.CreatePlaylist;
using Audiochan.Tests.Common.Mocks;
using Bogus;
using FluentValidation;
using FluentValidation.TestHelper;
using Moq;
using Xunit;

namespace Audiochan.Core.UnitTests.Validations.Playlists
{
    public class CreatePlaylistValidatorTests
    {
        private readonly IValidator<CreatePlaylistCommand> _validator;
        private readonly Mock<IUnitOfWork> _unitOfWork;

        public CreatePlaylistValidatorTests()
        {
            _unitOfWork = new UnitOfWorkMock().Create();
            _unitOfWork
                .Setup(x => x.Audios.ExistsAsync(It.IsAny<Expression<Func<Audio, bool>>>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);
            _validator = new CreatePlaylistCommandValidator(_unitOfWork.Object);
        }

        [Fact]
        public void ShouldFailWhenAudioIdsIsInvalid()
        {
            var faker = new Faker();
            _unitOfWork
                .Setup(x => x.Audios.ExistsAsync(It.IsAny<Expression<Func<Audio, bool>>>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);
            var result = _validator.TestValidate(new CreatePlaylistCommand
            {
                AudioIds = faker.Make(5, () => faker.Random.Guid()).ToList()
            });
            result.ShouldHaveValidationErrorFor(x => x.AudioIds);
        }
    }
}