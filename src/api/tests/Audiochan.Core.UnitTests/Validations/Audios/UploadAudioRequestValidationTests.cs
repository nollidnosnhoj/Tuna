using System.Threading.Tasks;
using Audiochan.Core.Audios;
using Audiochan.Core.Audios.Commands;
using Audiochan.Core.Common;
using Audiochan.Tests.Common.Builders;
using Bogus;
using FluentAssertions;
using FluentValidation;
using FluentValidation.TestHelper;
using Microsoft.Extensions.Options;
using NUnit.Framework;

namespace Audiochan.Core.UnitTests.Validations.Audios
{
    public class UploadAudioRequestValidationTests
    {
        private readonly IValidator<CreateUploadCommand> _validator;
        private readonly Randomizer _randomizer;

        public UploadAudioRequestValidationTests()
        {
            var options = Options.Create(new MediaStorageSettings
            {
                Audio = MediaStorageSettingBuilder.BuildAudioDefault()
            });
            _validator = new CreateUploadCommandValidator(options);
            _randomizer = new Randomizer();
        }

        [Test]
        public async Task ShouldValidateSuccessfully()
        {
            // Assign
            var request = new CreateUploadCommand
            {
                FileSize = _randomizer.Number(1, (int)MediaStorageSettingBuilder.MaxAudioSize),
                FileName = _randomizer.Word() + ".mp3"
            };

            // Act
            var result = await _validator.TestValidateAsync(request);

            // Assert
            result.IsValid.Should().BeTrue();
        }
        
        [Test]
        public async Task ShouldNotValidate_WhenRequiredFieldsAreMissing()
        {
            // Assign
            var request = new CreateUploadCommand();

            // Act
            var result = await _validator.TestValidateAsync(request);

            // Assert
            result.IsValid.Should().BeFalse();
            result.ShouldHaveValidationErrorFor(x => x.FileName);
            result.ShouldHaveValidationErrorFor(x => x.FileSize);
        }

        [Test]
        public async Task ShouldNotValidate_WhenFileNameHasNoExtension()
        {
            // Assign
            var request = new CreateUploadCommand {FileName = _randomizer.Word()};
            
            // Act
            var result = await _validator.TestValidateAsync(request);
            
            // Assert
            result.IsValid.Should().BeFalse();
            result.ShouldHaveValidationErrorFor(x => x.FileName);
        }
        
        [Test]
        public async Task ShouldNotValidate_WhenFileNameHaveInvalidContentType()
        {
            // Assign
            var request = new CreateUploadCommand {FileName = _randomizer.Word() + ".jpg"};
            
            // Act
            var result = await _validator.TestValidateAsync(request);
            
            // Assert
            result.IsValid.Should().BeFalse();
            result.ShouldHaveValidationErrorFor(x => x.FileName);
        }
        
        [Test]
        public async Task ShouldNotValidate_WhenFileSizeIsTooLarge()
        {
            // Assign
            var request = new CreateUploadCommand {FileSize = MediaStorageSettingBuilder.MaxAudioSize + 1};
            
            // Act
            var result = await _validator.TestValidateAsync(request);
            
            // Assert
            result.IsValid.Should().BeFalse();
            result.ShouldHaveValidationErrorFor(x => x.FileSize);
        }
    }
}