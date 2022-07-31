using Audiochan.Core.Audios.Commands;
using Audiochan.Tests.Common.Builders;
using Audiochan.Tests.Common.Fakers.Audios;
using FluentAssertions;
using FluentValidation;
using FluentValidation.TestHelper;
using Microsoft.Extensions.Options;
using Xunit;

namespace Audiochan.Core.UnitTests.Validations.Audios
{
    public class CreateAudioRequestValidationTests
    {
        private readonly AudioStorageSettings _audioStorageSettings;
        private readonly IValidator<CreateAudioCommand> _validator;

        public CreateAudioRequestValidationTests()
        {
            var options = Options.Create(new MediaStorageSettings
            {
                Audio = MediaStorageSettingBuilder.BuildAudioDefault()
            });
            _audioStorageSettings = options.Value.Audio;
            _validator = new CreateAudioCommandValidator(options);
        }

        [Fact]
        public void ShouldSuccessfullyValidateRequest()
        {
            var request = new CreateAudioCommandFaker().Generate();
            var result = _validator.TestValidate(request);
            result.IsValid.Should().BeTrue();
        }

        [Fact]
        public void ShouldBeInvalid_WhenUploadIdIsEmpty()
        {
            var request = new CreateAudioCommandFaker()
                .RuleFor(x => x.UploadId, string.Empty)
                .Generate();
            var result = _validator.TestValidate(request);
            result.ShouldHaveValidationErrorFor(x => x.UploadId);
        }

        [Fact]
        public void ShouldBeInvalid_WhenDurationIsEmpty()
        {
            var request = new CreateAudioCommandFaker()
                .RuleFor(x => x.Duration, () => default)
                .Generate();
            var result = _validator.TestValidate(request);
            result.ShouldHaveValidationErrorFor(x => x.Duration);
        }

        [Fact]
        public void ShouldBeInvalid_WhenFileSizeReachedOverLimit()
        {
            var maxSize = (int)_audioStorageSettings.MaximumFileSize;
            var request = new CreateAudioCommandFaker()
                .RuleFor(x => x.FileSize, maxSize + 100)
                .Generate();
            var result = _validator.TestValidate(request);
            result.ShouldHaveValidationErrorFor(x => x.FileSize);
        }

        [Fact]
        public void ShouldBeInvalid_WhenFileNameDoesNotHaveFileExtension()
        {
            var request = new CreateAudioCommandFaker()
                .RuleFor(x => x.FileName, "test")
                .Generate();
            var result = _validator.TestValidate(request);
            result.ShouldHaveValidationErrorFor(x => x.FileName);
        }
        
        [Fact]
        public void ShouldBeInvalid_WhenFileNameHasInvalidFileExtension()
        {
            var request = new CreateAudioCommandFaker()
                .RuleFor(x => x.FileName, f => f.System.FileName("jpg"))
                .Generate();
            var result = _validator.TestValidate(request);
            result.ShouldHaveValidationErrorFor(x => x.FileName);
        }
    }
}