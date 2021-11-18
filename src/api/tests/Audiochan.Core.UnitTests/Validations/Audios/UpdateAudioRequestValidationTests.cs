﻿using System.Collections.Generic;
using Audiochan.Core.Audios;
using Audiochan.Core.Audios.Commands;
using FluentValidation;
using FluentValidation.TestHelper;
using NUnit.Framework;

namespace Audiochan.Core.UnitTests.Validations.Audios
{
    public class UpdateAudioRequestValidationTests
    {
        private readonly IValidator<UpdateAudioCommand> _validator;

        public UpdateAudioRequestValidationTests()
        {
            _validator = new UpdateAudioCommandValidator();
        }

        [Test]
        public void CheckIfOverTenTagsIsInvalid()
        {
            var tags = new List<string>()
            {
                "word1", "word2", "word3", "word4", "word5", "word6", "word7", "word8", "word9", "word10", "word11"
            };

            var dto = new UpdateAudioCommand {Tags = tags};
            var result = _validator.TestValidate(dto);
            result.ShouldHaveValidationErrorFor(x => x.Tags);
        }

        [Test]
        public void CheckIfLessOrEqualToTenTagsIsValid()
        {
            var tags = new List<string>
            {
                "word1", "word2", "word3", "word4", "word5", "word6", "word7", "word8", "word9"
            };

            var dto = new UpdateAudioCommand {Tags = tags};
            var result = _validator.TestValidate(dto);
            result.ShouldNotHaveValidationErrorFor(x => x.Tags);
        }

        [Test]
        public void CheckIfNullTagsIsValid()
        {
            var dto = new UpdateAudioCommand();
            var result = _validator.TestValidate(dto);
            result.ShouldNotHaveValidationErrorFor(x => x.Tags);
        }

        [Test]
        public void CheckIfEmptyTagsIsValid()
        {
            var dto = new UpdateAudioCommand {Tags = new List<string>()};
            var result = _validator.TestValidate(dto);
            result.ShouldNotHaveValidationErrorFor(x => x.Tags);
        }
    }
}