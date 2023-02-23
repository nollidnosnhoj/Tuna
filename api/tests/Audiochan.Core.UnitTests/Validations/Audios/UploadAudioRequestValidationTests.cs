using System.Threading.Tasks;
using Audiochan.Core.Features.Upload.Commands;
using Bogus;
using FluentAssertions;
using FluentValidation;
using FluentValidation.TestHelper;
using Xunit;

namespace Audiochan.Core.UnitTests.Validations.Audios
{
    public class UploadAudioRequestValidationTests
    {
        private readonly IValidator<CreateAudioUploadCommand> _validator;
        private readonly Randomizer _randomizer;

        public UploadAudioRequestValidationTests()
        {
            _validator = new CreateAudioUploadCommandValidator();
            _randomizer = new Randomizer();
        }

        [Fact]
        public async Task ShouldValidateSuccessfully()
        {
            // Assign
            var request = new CreateAudioUploadCommand(
                _randomizer.Word() + ".mp3",
                _randomizer.Number(1, MediaConfigurationConstants.AUDIO_MAX_FILE_SIZE),
                1);

            // Act
            var result = await _validator.TestValidateAsync(request);

            // Assert
            result.IsValid.Should().BeTrue();
        }

        [Fact]
        public async Task ShouldNotValidate_WhenFileNameHasNoExtension()
        {
            // Assign
            var request = new CreateAudioUploadCommand(
                _randomizer.Word(), 
                _randomizer.Number(1, MediaConfigurationConstants.AUDIO_MAX_FILE_SIZE), 
                1);
            
            // Act
            var result = await _validator.TestValidateAsync(request);
            
            // Assert
            result.IsValid.Should().BeFalse();
            result.ShouldHaveValidationErrorFor(x => x.FileName);
        }
        
        [Fact]
        public async Task ShouldNotValidate_WhenFileNameHaveInvalidContentType()
        {
            // Assign
            var request = new CreateAudioUploadCommand(
                _randomizer.Word() + ".jpg",
                _randomizer.Number(1, MediaConfigurationConstants.AUDIO_MAX_FILE_SIZE),
                1);
            
            // Act
            var result = await _validator.TestValidateAsync(request);
            
            // Assert
            result.IsValid.Should().BeFalse();
            result.ShouldHaveValidationErrorFor(x => x.FileName);
        }
        
        [Fact]
        public async Task ShouldNotValidate_WhenFileSizeIsTooLarge()
        {
            // Assign
            var request = new CreateAudioUploadCommand(
                _randomizer.Word() + ".mp3",
                MediaConfigurationConstants.AUDIO_MAX_FILE_SIZE + 1,
                1);
            
            // Act
            var result = await _validator.TestValidateAsync(request);
            
            // Assert
            result.IsValid.Should().BeFalse();
            result.ShouldHaveValidationErrorFor(x => x.FileSize);
        }
    }
}