using System.Threading.Tasks;
using Audiochan.Core.Common;
using Audiochan.Core.Features.Audios.CreateAudioUploadUrl;
using Audiochan.Tests.Common.Builders;
using Bogus;
using FluentAssertions;
using FluentValidation;
using FluentValidation.TestHelper;
using Microsoft.Extensions.Options;
using Xunit;

namespace Audiochan.Core.UnitTests.Validations.Audios
{
    public class UploadAudioRequestValidationTests
    {
        private readonly IValidator<CreateAudioUploadUrlCommand> _validator;
        private readonly Randomizer _randomizer;

        public UploadAudioRequestValidationTests()
        {
            var options = Options.Create(new MediaStorageSettings
            {
                Audio = MediaStorageSettingBuilder.BuildAudioDefault()
            });
            _validator = new CreateAudioUploadUrlCommandValidator(options);
            _randomizer = new Randomizer();
        }

        [Fact]
        public async Task ShouldValidateSuccessfully()
        {
            // Assign
            var request = new CreateAudioUploadUrlCommand
            {
                FileSize = _randomizer.Number(1, (int)MediaStorageSettingBuilder.MaxAudioSize),
                FileName = _randomizer.Word() + ".mp3"
            };

            // Act
            var result = await _validator.TestValidateAsync(request);

            // Assert
            result.IsValid.Should().BeTrue();
        }
        
        [Fact]
        public async Task ShouldNotValidate_WhenRequiredFieldsAreMissing()
        {
            // Assign
            var request = new CreateAudioUploadUrlCommand();

            // Act
            var result = await _validator.TestValidateAsync(request);

            // Assert
            result.IsValid.Should().BeFalse();
            result.ShouldHaveValidationErrorFor(x => x.FileName);
            result.ShouldHaveValidationErrorFor(x => x.FileSize);
        }

        [Fact]
        public async Task ShouldNotValidate_WhenFileNameHasNoExtension()
        {
            // Assign
            var request = new CreateAudioUploadUrlCommand {FileName = _randomizer.Word()};
            
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
            var request = new CreateAudioUploadUrlCommand {FileName = _randomizer.Word() + ".jpg"};
            
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
            var request = new CreateAudioUploadUrlCommand {FileSize = MediaStorageSettingBuilder.MaxAudioSize + 1};
            
            // Act
            var result = await _validator.TestValidateAsync(request);
            
            // Assert
            result.IsValid.Should().BeFalse();
            result.ShouldHaveValidationErrorFor(x => x.FileSize);
        }
    }
}