using System.Threading.Tasks;
using Audiochan.Core.Features.Upload.Commands.CreateUpload;
using Audiochan.Tests.Common.Builders;
using Audiochan.Tests.Common.Mocks;
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
            var request = new GenerateUploadLinkCommand(
                _randomizer.Word() + ".mp3",
                _randomizer.Number(1, (int)MediaStorageSettingBuilder.MaxAudioSize),
                ClaimsPrincipalMock.CreateFakeUser(_randomizer));

            // Act
            var result = await _validator.TestValidateAsync(request);

            // Assert
            result.IsValid.Should().BeTrue();
        }

        [Fact]
        public async Task ShouldNotValidate_WhenFileNameHasNoExtension()
        {
            // Assign
            var request = new GenerateUploadLinkCommand(
                _randomizer.Word(), 
                _randomizer.Number(1, (int)MediaStorageSettingBuilder.MaxAudioSize), 
                ClaimsPrincipalMock.CreateFakeUser(_randomizer));
            
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
            var request = new GenerateUploadLinkCommand(
                _randomizer.Word() + ".jpg",
                _randomizer.Number(1, (int)MediaStorageSettingBuilder.MaxAudioSize),
                ClaimsPrincipalMock.CreateFakeUser(_randomizer));
            
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
            var request = new GenerateUploadLinkCommand(
                _randomizer.Word() + ".mp3",
                MediaStorageSettingBuilder.MaxAudioSize + 1,
                ClaimsPrincipalMock.CreateFakeUser(_randomizer));
            
            // Act
            var result = await _validator.TestValidateAsync(request);
            
            // Assert
            result.IsValid.Should().BeFalse();
            result.ShouldHaveValidationErrorFor(x => x.FileSize);
        }
    }
}