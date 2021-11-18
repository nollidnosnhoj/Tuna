using Audiochan.Core.Common;
using Audiochan.Core.Users;
using Audiochan.Core.Users.Commands;
using FluentValidation;
using FluentValidation.TestHelper;
using Microsoft.Extensions.Options;
using NUnit.Framework;

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

        [Test]
        public void PasswordRequired()
        {
            var req = new UpdatePasswordCommand {NewPassword = ""};
            var validationResult = _validator.TestValidate(req);
            validationResult
                .ShouldHaveValidationErrorFor(x => x.NewPassword);
        }

        [Theory]
        [TestCase("thisdoesnothavedigits")]
        public void PasswordRequireDigits(string password)
        {
            var req = new UpdatePasswordCommand {NewPassword = password};

            var validationResult = _validator.TestValidate(req);

            validationResult
                .ShouldHaveValidationErrorFor(x => x.NewPassword)
                .WithErrorCode(ValidationErrorCodes.Password.DIGITS);
        }

        [Theory]
        [TestCase("OMEGALUL4HEAD")]
        public void PasswordRequireLowercase(string password)
        {
            var req = new UpdatePasswordCommand {NewPassword = password};

            var validationResult = _validator.TestValidate(req);

            validationResult
                .ShouldHaveValidationErrorFor(x => x.NewPassword)
                .WithErrorCode(ValidationErrorCodes.Password.LOWERCASE);
        }

        [Theory]
        [TestCase("omegalul4head")]
        public void PasswordRequireUppercase(string password)
        {
            var req = new UpdatePasswordCommand {NewPassword = password};

            var validationResult = _validator.TestValidate(req);

            validationResult
                .ShouldHaveValidationErrorFor(x => x.NewPassword)
                .WithErrorCode(ValidationErrorCodes.Password.UPPERCASE);
        }

        [Theory]
        [TestCase("lkdsfhlksdjflksdjflks")]
        public void PasswordRequireNonAlphanumeric(string password)
        {
            var req = new UpdatePasswordCommand {NewPassword = password};

            var validationResult = _validator.TestValidate(req);

            validationResult
                .ShouldHaveValidationErrorFor(x => x.NewPassword)
                .WithErrorCode(ValidationErrorCodes.Password.NON_ALPHANUMERIC);
        }

        [Theory]
        [TestCase("no")]
        public void PasswordRequireLength(string password)
        {
            var req = new UpdatePasswordCommand {NewPassword = password};

            var validationResult = _validator.TestValidate(req);

            validationResult
                .ShouldHaveValidationErrorFor(x => x.NewPassword)
                .WithErrorCode(ValidationErrorCodes.Password.LENGTH);
        }
    }
}