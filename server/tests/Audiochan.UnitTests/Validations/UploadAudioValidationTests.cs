using System.Collections.Generic;
using System.Threading.Tasks;
using Audiochan.Core.Common.Settings;
using Audiochan.Core.Features.Audios.UploadAudio;
using Bogus;
using FluentAssertions;
using FluentValidation;
using FluentValidation.TestHelper;
using Microsoft.Extensions.Options;
using Xunit;

namespace Audiochan.UnitTests.Validations
{
    public class UploadAudioValidationTests
    {
        private readonly IValidator<UploadAudioRequest> _validator;
        private readonly Randomizer _randomizer;

        public UploadAudioValidationTests()
        {
            var options = Options.Create(new MediaStorageSettings
            {
                Audio = new MediaStorageSettings.StorageSettings
                {
                    Container = "audios",
                    ValidContentTypes = new List<string>
                    {
                        "audio/mp3",
                        "audio/mpeg",
                        "audio/ogg"
                    },
                    MaximumFileSize = 262_144_000
                }
            });
            _validator = new UploadAudioRequestValidator(options);
            _randomizer = new Randomizer();
        }

        [Fact]
        public async Task ShouldValidateSuccessfully()
        {
            // Assign
            var request = new UploadAudioRequest
            {
                Duration = _randomizer.Number(300),
                FileSize = _randomizer.Number(1, 262144000),
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
            var request = new UploadAudioRequest();

            // Act
            var result = await _validator.TestValidateAsync(request);

            // Assert
            result.IsValid.Should().BeFalse();
            result.ShouldHaveValidationErrorFor(x => x.Duration);
            result.ShouldHaveValidationErrorFor(x => x.FileName);
            result.ShouldHaveValidationErrorFor(x => x.FileSize);
        }

        [Fact]
        public async Task ShouldNotValidate_WhenFileNameHasNoExtension()
        {
            // Assign
            var request = new UploadAudioRequest {FileName = _randomizer.Word()};
            
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
            var request = new UploadAudioRequest {FileName = _randomizer.Word() + ".jpg"};
            
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
            var request = new UploadAudioRequest {FileSize = 262_144_000 + 1};
            
            // Act
            var result = await _validator.TestValidateAsync(request);
            
            // Assert
            result.IsValid.Should().BeFalse();
            result.ShouldHaveValidationErrorFor(x => x.FileSize);
        }
    }
}