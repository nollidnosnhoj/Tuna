using Audiochan.Core.Features.Audios.Commands;
using Audiochan.Tests.Common.Fakers.Audios;
using FluentAssertions;
using FluentValidation;
using FluentValidation.TestHelper;
using Xunit;

namespace Audiochan.Core.UnitTests.Validations.Audios
{
    public class CreateAudioRequestValidationTests
    {
        private readonly IValidator<CreateAudioCommand> _validator;

        public CreateAudioRequestValidationTests()
        {
            _validator = new CreateAudioCommandValidator();
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
            var request = new CreateAudioCommandFaker()
                .RuleFor(x => x.FileSize, MediaConfigurationConstants.AUDIO_MAX_FILE_SIZE + 100)
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