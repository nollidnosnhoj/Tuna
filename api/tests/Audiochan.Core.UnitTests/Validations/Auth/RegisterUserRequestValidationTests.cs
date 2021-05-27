using Audiochan.API.Features.Auth.Register;
using Audiochan.Core.Constants;
using Audiochan.Core.Settings;
using FluentValidation;
using FluentValidation.TestHelper;
using Microsoft.Extensions.Options;
using Xunit;

namespace Audiochan.Core.UnitTests.Validations.Auth
{
    public class RegisterUserRequestValidationTests
    {
        private readonly IValidator<RegisterUserRequest> _validator;

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
            _validator = new RegisterUserRequestValidator(options);
        }

        [Fact]
        public void UsernameRequired()
        {
            var req = new RegisterUserRequest();
            _validator.TestValidate(req)
                .ShouldHaveValidationErrorFor(x => x.Username);
        }

        [Fact]
        public void UsernameRequiredCharacters()
        {
            var req = new RegisterUserRequest {Username = "@pplesAnd&&&&"};
            _validator.TestValidate(req)
                .ShouldHaveValidationErrorFor(x => x.Username)
                .WithErrorCode(ValidationErrorCodes.Username.Characters);
        }

        [Fact]
        public void PasswordRequired()
        {
            var req = new RegisterUserRequest();
            var validationResult = _validator.TestValidate(req);
            validationResult
                .ShouldHaveValidationErrorFor(x => x.Password);
        }

        [Fact]
        public void PasswordRequireDigits()
        {
            var req = new RegisterUserRequest {Password = "thisdoesnothavedigits"};

            var validationResult = _validator.TestValidate(req);

            validationResult
                .ShouldHaveValidationErrorFor(x => x.Password)
                .WithErrorCode(ValidationErrorCodes.Password.Digits);
        }

        [Fact]
        public void PasswordRequireLowercase()
        {
            var req = new RegisterUserRequest {Password = "OMEGALUL4HEAD"};

            var validationResult = _validator.TestValidate(req);

            validationResult
                .ShouldHaveValidationErrorFor(x => x.Password)
                .WithErrorCode(ValidationErrorCodes.Password.Lowercase);
        }

        [Fact]
        public void PasswordRequireUppercase()
        {
            var req = new RegisterUserRequest {Password = "omegalul4head"};

            var validationResult = _validator.TestValidate(req);

            validationResult
                .ShouldHaveValidationErrorFor(x => x.Password)
                .WithErrorCode(ValidationErrorCodes.Password.Uppercase);
        }

        [Fact]
        public void PasswordRequireNonAlphanumeric()
        {
            var req = new RegisterUserRequest {Password = "lkdsfhlksdjflksdjflks"};

            var validationResult = _validator.TestValidate(req);

            validationResult
                .ShouldHaveValidationErrorFor(x => x.Password)
                .WithErrorCode(ValidationErrorCodes.Password.NonAlphanumeric);
        }

        [Fact]
        public void PasswordRequireLength()
        {
            var req = new RegisterUserRequest {Password = "no"};

            var validationResult = _validator.TestValidate(req);

            validationResult
                .ShouldHaveValidationErrorFor(x => x.Password)
                .WithErrorCode(ValidationErrorCodes.Password.Length);
        }
    }
}