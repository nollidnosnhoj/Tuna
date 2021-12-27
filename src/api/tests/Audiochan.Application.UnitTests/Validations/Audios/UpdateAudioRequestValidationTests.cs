using System;
using Audiochan.Application.Features.Audios.Commands.UpdateAudio;
using FluentValidation;
using FluentValidation.TestHelper;
using Xunit;

namespace Audiochan.Application.UnitTests.Validations.Audios
{
    public class UpdateAudioRequestValidationTests
    {
        private readonly IValidator<UpdateAudioCommand> _validator;

        public UpdateAudioRequestValidationTests()
        {
            _validator = new UpdateAudioCommandValidator();
        }

        [Fact]
        public void CheckIfOverTenTagsIsInvalid()
        {
            var tags = new[]
            {
                "word1", "word2", "word3", "word4", "word5", "word6", "word7", "word8", "word9", "word10", "word11"
            };

            var dto = new UpdateAudioCommand(1, null, null, tags);
            var result = _validator.TestValidate(dto);
            result.ShouldHaveValidationErrorFor(x => x.Tags);
        }

        [Fact]
        public void CheckIfLessOrEqualToTenTagsIsValid()
        {
            var tags = new[]
            {
                "word1", "word2", "word3", "word4", "word5", "word6", "word7", "word8", "word9"
            };

            var dto = new UpdateAudioCommand(1, null, null, tags);
            var result = _validator.TestValidate(dto);
            result.ShouldNotHaveValidationErrorFor(x => x.Tags);
        }

        [Fact]
        public void CheckIfNullTagsIsValid()
        {
            var dto = new UpdateAudioCommand(1, null, null, null);
            var result = _validator.TestValidate(dto);
            result.ShouldNotHaveValidationErrorFor(x => x.Tags);
        }

        [Fact]
        public void CheckIfEmptyTagsIsValid()
        {
            var dto = new UpdateAudioCommand(1, null, null, Array.Empty<string>());
            var result = _validator.TestValidate(dto);
            result.ShouldNotHaveValidationErrorFor(x => x.Tags);
        }
    }
}