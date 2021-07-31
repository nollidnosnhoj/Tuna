﻿using Audiochan.Core.Common.Constants;
using Audiochan.Core.Common.Settings;
using Audiochan.Core.Features.Users.UpdatePassword;
using FluentValidation;
using FluentValidation.TestHelper;
using Microsoft.Extensions.Options;
using Xunit;

namespace Audiochan.Core.UnitTests.Validations.Users
{
    public class UpdatePasswordCommandValidatorTests
    {
        private readonly IValidator<UpdatePasswordCommand> _validator;

        public UpdatePasswordCommandValidatorTests()
        {
            var options = Options.Create(new IdentitySettings());
            _validator = new UpdatePasswordCommandValidator(options);
        }

        [Fact]
        public void PasswordRequired()
        {
            var req = new UpdatePasswordCommand {NewPassword = ""};
            var validationResult = _validator.TestValidate(req);
            validationResult
                .ShouldHaveValidationErrorFor(x => x.NewPassword);
        }

        [Theory]
        [InlineData("thisdoesnothavedigits")]
        public void PasswordRequireDigits(string password)
        {
            var req = new UpdatePasswordCommand {NewPassword = password};

            var validationResult = _validator.TestValidate(req);

            validationResult
                .ShouldHaveValidationErrorFor(x => x.NewPassword)
                .WithErrorCode(ValidationErrorCodes.Password.Digits);
        }

        [Theory]
        [InlineData("OMEGALUL4HEAD")]
        public void PasswordRequireLowercase(string password)
        {
            var req = new UpdatePasswordCommand {NewPassword = password};

            var validationResult = _validator.TestValidate(req);

            validationResult
                .ShouldHaveValidationErrorFor(x => x.NewPassword)
                .WithErrorCode(ValidationErrorCodes.Password.Lowercase);
        }

        [Theory]
        [InlineData("omegalul4head")]
        public void PasswordRequireUppercase(string password)
        {
            var req = new UpdatePasswordCommand {NewPassword = password};

            var validationResult = _validator.TestValidate(req);

            validationResult
                .ShouldHaveValidationErrorFor(x => x.NewPassword)
                .WithErrorCode(ValidationErrorCodes.Password.Uppercase);
        }

        [Theory]
        [InlineData("lkdsfhlksdjflksdjflks")]
        public void PasswordRequireNonAlphanumeric(string password)
        {
            var req = new UpdatePasswordCommand {NewPassword = password};

            var validationResult = _validator.TestValidate(req);

            validationResult
                .ShouldHaveValidationErrorFor(x => x.NewPassword)
                .WithErrorCode(ValidationErrorCodes.Password.NonAlphanumeric);
        }

        [Theory]
        [InlineData("no")]
        public void PasswordRequireLength(string password)
        {
            var req = new UpdatePasswordCommand {NewPassword = password};

            var validationResult = _validator.TestValidate(req);

            validationResult
                .ShouldHaveValidationErrorFor(x => x.NewPassword)
                .WithErrorCode(ValidationErrorCodes.Password.Length);
        }
    }
}