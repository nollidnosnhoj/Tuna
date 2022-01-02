using System.Threading.Tasks;
using Audiochan.Application.Features.Audios.Commands.CreateUpload;
using Audiochan.Tests.Common.Builders;
using Bogus;
using FluentAssertions;
using FluentValidation;
using FluentValidation.TestHelper;
using Microsoft.Extensions.Options;
using Xunit;

namespace Audiochan.Application.UnitTests.Validations.Audios
{
    public class UploadAudioRequestValidationTests
    {
        private readonly IValidator<GenerateUploadLinkCommand> _validator;
        private readonly Randomizer _randomizer;

        public UploadAudioRequestValidationTests()
        {
            var options = Options.Create(new MediaStorageSettings
            {
                Audio = MediaStorageSettingBuilder.BuildAudioDefault()
            });
            _validator = new GenerateUploadLinkCommandValidator(options);
            _randomizer = new Randomizer();
        }

        [Fact]
        public async Task ShouldValidateSuccessfully()
        {
            // Assign
            var fileSize = _randomizer.Number(1, (int) MediaStorageSettingBuilder.MaxAudioSize);
            var fileName = _randomizer.Word() + ".mp3";
            var request = new GenerateUploadLinkCommand(fileName, fileSize);

            // Act
            var result = await _validator.TestValidateAsync(request);

            // Assert
            result.IsValid.Should().BeTrue();
        }
        
        [Fact]
        public async Task ShouldNotValidate_WhenRequiredFieldsAreMissing()
        {
            // Assign
            var request = new GenerateUploadLinkCommand("", 0);

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
            var request = new GenerateUploadLinkCommand(_randomizer.Word(), 100);
            
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
            var request = new GenerateUploadLinkCommand(_randomizer.Word() + ".jpg", 100);
            
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
            var request = new GenerateUploadLinkCommand("test.mp3", MediaStorageSettingBuilder.MaxAudioSize + 1);
            
            // Act
            var result = await _validator.TestValidateAsync(request);
            
            // Assert
            result.IsValid.Should().BeFalse();
            result.ShouldHaveValidationErrorFor(x => x.FileSize);
        }
    }
}