using System.Collections.Generic;
using Audiochan.Core.Features.Audios.UpdateAudio;
using FluentValidation;
using FluentValidation.TestHelper;
using Xunit;

namespace Audiochan.UnitTests.Validations
{
    public class UpdateAudioValidationTests
    {
        private readonly IValidator<UpdateAudioRequest> _validator;

        public UpdateAudioValidationTests()
        {
            _validator = new UpdateAudioRequestValidator();
        }

        [Fact]
        public void CheckIfOverTenTagsIsInvalid()
        {
            var tags = new List<string>()
            {
                "word1", "word2", "word3", "word4", "word5", "word6", "word7", "word8", "word9", "word10", "word11"
            };

            var dto = new UpdateAudioRequest {Tags = tags};
            var result = _validator.TestValidate(dto);
            result.ShouldHaveValidationErrorFor(x => x.Tags);
        }

        [Fact]
        public void CheckIfLessOrEqualToTenTagsIsValid()
        {
            var tags = new List<string>
            {
                "word1", "word2", "word3", "word4", "word5", "word6", "word7", "word8", "word9"
            };

            var dto = new UpdateAudioRequest {Tags = tags};
            var result = _validator.TestValidate(dto);
            result.ShouldNotHaveValidationErrorFor(x => x.Tags);
        }

        [Fact]
        public void CheckIfNullTagsIsValid()
        {
            var dto = new UpdateAudioRequest();
            var result = _validator.TestValidate(dto);
            result.ShouldNotHaveValidationErrorFor(x => x.Tags);
        }

        [Fact]
        public void CheckIfEmptyTagsIsValid()
        {
            var dto = new UpdateAudioRequest {Tags = new List<string>()};
            var result = _validator.TestValidate(dto);
            result.ShouldNotHaveValidationErrorFor(x => x.Tags);
        }
    }
}