using System.Collections.Generic;
using Audiochan.Core.Common.Options;
using Audiochan.Core.Features.Audios.Models;
using Audiochan.Core.Features.Audios.Validators;
using FluentValidation;
using FluentValidation.TestHelper;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;

namespace Audiochan.UnitTests.Validations
{
    public class UploadAudioValidationTests
    {
        private readonly IValidator<UploadAudioRequest> _validator;
        private readonly Mock<IFormFile> _fileMock;

        public UploadAudioValidationTests()
        {
            var options = Options.Create(new AudiochanOptions
            {
                AudioUploadOptions = new AudiochanOptions.UploadOptions
                {
                    FileExtensions = new List<string>
                    {
                        "mp3", "ogg", "aac"
                    },
                    FileSize = 262144000
                }
            });
            _validator = new UploadAudioRequestValidator(options);
            _fileMock = new Mock<IFormFile>();
        }
        
        [Fact]
        public void CheckIfTitleIsValidWhenEmpty()
        {
            var result = _validator.TestValidate(new UploadAudioRequest{Title=""});
            result.ShouldNotHaveValidationErrorFor(x => x.Title);
        }

        [Fact]
        public void CheckIfFileIsInvalidWhenEmpty()
        {
            var result = _validator.TestValidate(new UploadAudioRequest {File = null!});
            result.ShouldHaveValidationErrorFor(x => x.File);
        }

        [Fact]
        public void CheckIfFileIsInvalidWhenInvalidContentType()
        {
            _fileMock.Setup(x => x.FileName).Returns("test.jpg");
            _fileMock.Setup(x => x.Length).Returns(50);
            var result = _validator.TestValidate(new UploadAudioRequest {File = _fileMock.Object});
            result.ShouldHaveValidationErrorFor(x => x.File);
        }

        [Fact]
        public void CheckIfFileIsInvalidWhenFileLengthExceeded()
        {
            _fileMock.Setup(x => x.FileName).Returns("test.ogg");
            _fileMock.Setup(x => x.Length).Returns(2000000 * 1000 + 1);
            var result = _validator.TestValidate(new UploadAudioRequest {File = _fileMock.Object});
            result.ShouldHaveValidationErrorFor(x => x.File);
        }

        [Fact]
        public void CheckIfFileIsValid()
        {
            _fileMock.Setup(x => x.FileName).Returns("test.mp3");
            _fileMock.Setup(x => x.Length).Returns(2000000);
            var result = _validator.TestValidate(new UploadAudioRequest {File = _fileMock.Object});
            result.ShouldNotHaveValidationErrorFor(x => x.File);
        }

        [Fact]
        public void CheckIfOverTenTagsIsInvalid()
        {
            var tags = new List<string?>
            {
                "word1", "word2", "word3", "word4", "word5", "word6", "word7", "word8", "word9", "word10", "word11"
            };

            var dto = new UploadAudioRequest {Tags = tags};
            var result = _validator.TestValidate(dto);
            result.ShouldHaveValidationErrorFor(x => x.Tags);
        }
        
        [Fact]
        public void CheckIfLessOrEqualToTenTagsIsValid()
        {
            var tags = new List<string?>
            {
                "word1", "word2", "word3", "word4", "word5", "word6", "word7", "word8", "word9"
            };

            var dto = new UploadAudioRequest {Tags = tags};
            var result = _validator.TestValidate(dto);
            result.ShouldNotHaveValidationErrorFor(x => x.Tags);
        }
        
        [Fact]
        public void CheckIfNullTagsIsValid()
        {
            var dto = new UploadAudioRequest();
            var result = _validator.TestValidate(dto);
            result.ShouldNotHaveValidationErrorFor(x => x.Tags);
        }
        
        [Fact]
        public void CheckIfEmptyTagsIsValid()
        {
            var dto = new UploadAudioRequest{Tags = new List<string?>()};
            var result = _validator.TestValidate(dto);
            result.ShouldNotHaveValidationErrorFor(x => x.Tags);
        }
    }
}