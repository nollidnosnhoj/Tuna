using System.Threading;
using Audiochan.Core.Common.Settings;
using Audiochan.Core.Features.Audios.CreateAudio;
using Audiochan.Core.Services;
using Audiochan.Tests.Common.Builders;
using Audiochan.Tests.Common.Fakers.Audios;
using FluentAssertions;
using FluentValidation;
using FluentValidation.TestHelper;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;

namespace Audiochan.Core.UnitTests.Validations.Audios
{
    public class CreateAudioRequestValidationTests
    {
        private readonly MediaStorageSettings.StorageSettings _audioStorageSettings;
        private readonly IValidator<CreateAudioCommand> _validator;

        public CreateAudioRequestValidationTests()
        {
            var options = Options.Create(new MediaStorageSettings
            {
                Audio = MediaStorageSettingBuilder.BuildAudioDefault()
            });
            _audioStorageSettings = options.Value.Audio;
            var storageMock = new Mock<IStorageService>();
            storageMock.Setup(x => x.ExistsAsync(
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);
            _validator = new CreateAudioCommandValidator(options, storageMock.Object);
        }

        [Fact]
        public void ShouldSuccessfullyValidateRequest()
        {
            var request = new CreateAudioRequestFaker().Generate();
            var result = _validator.TestValidate(request);
            result.IsValid.Should().BeTrue();
        }

        [Fact]
        public void ShouldBeInvalid_WhenUploadIdIsEmpty()
        {
            var request = new CreateAudioRequestFaker()
                .RuleFor(x => x.UploadId, string.Empty)
                .Generate();
            var result = _validator.TestValidate(request);
            result.ShouldHaveValidationErrorFor(x => x.UploadId);
        }

        [Fact]
        public void ShouldBeInvalid_WhenDurationIsEmpty()
        {
            var request = new CreateAudioRequestFaker()
                .RuleFor(x => x.Duration, () => default)
                .Generate();
            var result = _validator.TestValidate(request);
            result.ShouldHaveValidationErrorFor(x => x.Duration);
        }

        [Fact]
        public void ShouldBeInvalid_WhenFileSizeReachedOverLimit()
        {
            var maxSize = (int)_audioStorageSettings.MaximumFileSize;
            var request = new CreateAudioRequestFaker()
                .RuleFor(x => x.FileSize, maxSize + 100)
                .Generate();
            var result = _validator.TestValidate(request);
            result.ShouldHaveValidationErrorFor(x => x.FileSize);
        }

        [Fact]
        public void ShouldBeInvalid_WhenFileNameDoesNotHaveFileExtension()
        {
            var request = new CreateAudioRequestFaker()
                .RuleFor(x => x.FileName, "test")
                .Generate();
            var result = _validator.TestValidate(request);
            result.ShouldHaveValidationErrorFor(x => x.FileName);
        }
        
        [Fact]
        public void ShouldBeInvalid_WhenFileNameHasInvalidFileExtension()
        {
            var request = new CreateAudioRequestFaker()
                .RuleFor(x => x.FileName, f => f.System.FileName("jpg"))
                .Generate();
            var result = _validator.TestValidate(request);
            result.ShouldHaveValidationErrorFor(x => x.FileName);
        }
    }
}