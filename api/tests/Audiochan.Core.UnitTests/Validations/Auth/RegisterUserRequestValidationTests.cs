using Audiochan.Core.Common.Constants;
using Audiochan.Core.Common.Settings;
using Audiochan.Core.Features.Auth.CreateUser;
using FluentValidation;
using FluentValidation.TestHelper;
using Microsoft.Extensions.Options;
using Xunit;

namespace Audiochan.Core.UnitTests.Validations.Auth
{
    public class RegisterUserRequestValidationTests
    {
        private readonly IValidator<CreateUserCommand> _validator;

        public RegisterUserRequestValidationTests()
        {
            var options = Options.Create(new IdentitySettings
            {
                PasswordSettings = new IdentitySettings.PasswordRules
                {
                    RequiresDigit = true,
                    RequiresLowercase = true,
                    RequiresUppercase = true,
                    RequiresNonAlphanumeric = true,
                    MinimumLength = 5
                }
            });
            _validator = new CreateUserCommandValidator(options);
        }

        [Fact]
        public void UsernameRequired()
        {
            var req = new CreateUserCommand();
            _validator.TestValidate(req)
                .ShouldHaveValidationErrorFor(x => x.Username);
        }

        [Fact]
        public void UsernameRequiredCharacters()
        {
            var req = new CreateUserCommand {Username = "@pplesAnd&&&&"};
            _validator.TestValidate(req)
                .ShouldHaveValidationErrorFor(x => x.Username)
                .WithErrorCode(ValidationErrorCodes.Username.Characters);
        }

        [Fact]
        public void PasswordRequired()
        {
            var req = new CreateUserCommand();
            var validationResult = _validator.TestValidate(req);
            validationResult
                .ShouldHaveValidationErrorFor(x => x.Password);
        }

        [Fact]
        public void PasswordRequireDigits()
        {
            var req = new CreateUserCommand {Password = "thisdoesnothavedigits"};

            var validationResult = _validator.TestValidate(req);

            validationResult
                .ShouldHaveValidationErrorFor(x => x.Password)
                .WithErrorCode(ValidationErrorCodes.Password.Digits);
        }

        [Fact]
        public void PasswordRequireLowercase()
        {
            var req = new CreateUserCommand {Password = "OMEGALUL4HEAD"};

            var validationResult = _validator.TestValidate(req);

            validationResult
                .ShouldHaveValidationErrorFor(x => x.Password)
                .WithErrorCode(ValidationErrorCodes.Password.Lowercase);
        }

        [Fact]
        public void PasswordRequireUppercase()
        {
            var req = new CreateUserCommand {Password = "omegalul4head"};

            var validationResult = _validator.TestValidate(req);

            validationResult
                .ShouldHaveValidationErrorFor(x => x.Password)
                .WithErrorCode(ValidationErrorCodes.Password.Uppercase);
        }

        [Fact]
        public void PasswordRequireNonAlphanumeric()
        {
            var req = new CreateUserCommand {Password = "lkdsfhlksdjflksdjflks"};

            var validationResult = _validator.TestValidate(req);

            validationResult
                .ShouldHaveValidationErrorFor(x => x.Password)
                .WithErrorCode(ValidationErrorCodes.Password.NonAlphanumeric);
        }

        [Fact]
        public void PasswordRequireLength()
        {
            var req = new CreateUserCommand {Password = "no"};

            var validationResult = _validator.TestValidate(req);

            validationResult
                .ShouldHaveValidationErrorFor(x => x.Password)
                .WithErrorCode(ValidationErrorCodes.Password.Length);
        }
    }
}