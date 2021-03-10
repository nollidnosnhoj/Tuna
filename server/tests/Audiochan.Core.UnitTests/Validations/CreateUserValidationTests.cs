using Audiochan.Core.Common.Constants;
using Audiochan.Core.Common.Options;
using Audiochan.Core.Features.Auth.Register;
using FluentValidation;
using FluentValidation.TestHelper;
using Microsoft.Extensions.Options;
using Xunit;

namespace Audiochan.Core.UnitTests.Validations
{
    public class CreateUserValidationTests
    {
        private readonly IValidator<RegisterCommand> _validator;

        public CreateUserValidationTests()
        {
            var options = Options.Create(new IdentityOptions
            {
                PasswordRequiresDigit = true,
                PasswordRequiresLowercase = true,
                PasswordRequiresUppercase = true,
                PasswordRequiresNonAlphanumeric = true,
                PasswordMinimumLength = 5
            });
            _validator = new RegisterCommandValidator(options);
        }

        [Fact]
        public void UsernameRequired()
        {
            var req = new RegisterCommand();
            _validator.TestValidate(req)
                .ShouldHaveValidationErrorFor(x => x.Username);
        }

        [Fact]
        public void UsernameRequiredCharacters()
        {
            var req = new RegisterCommand {Username = "@pplesAnd&&&&"};
            _validator.TestValidate(req)
                .ShouldHaveValidationErrorFor(x => x.Username)
                .WithErrorCode(ValidationErrorCodes.Username.RequireCharacters);
        }

        [Fact]
        public void PasswordRequired()
        {
            var req = new RegisterCommand();
            var validationResult = _validator.TestValidate(req);
            validationResult
                .ShouldHaveValidationErrorFor(x => x.Password);
        }

        [Fact]
        public void PasswordRequireDigits()
        {
            var req = new RegisterCommand {Password = "thisdoesnothavedigits"};

            var validationResult = _validator.TestValidate(req);

            validationResult
                .ShouldHaveValidationErrorFor(x => x.Password)
                .WithErrorCode(ValidationErrorCodes.Password.RequireDigits);
        }

        [Fact]
        public void PasswordRequireLowercase()
        {
            var req = new RegisterCommand {Password = "OMEGALUL4HEAD"};

            var validationResult = _validator.TestValidate(req);

            validationResult
                .ShouldHaveValidationErrorFor(x => x.Password)
                .WithErrorCode(ValidationErrorCodes.Password.RequireLowercase);
        }

        [Fact]
        public void PasswordRequireUppercase()
        {
            var req = new RegisterCommand {Password = "omegalul4head"};

            var validationResult = _validator.TestValidate(req);

            validationResult
                .ShouldHaveValidationErrorFor(x => x.Password)
                .WithErrorCode(ValidationErrorCodes.Password.RequireUppercase);
        }

        [Fact]
        public void PasswordRequireNonAlphanumeric()
        {
            var req = new RegisterCommand {Password = "lkdsfhlksdjflksdjflks"};

            var validationResult = _validator.TestValidate(req);

            validationResult
                .ShouldHaveValidationErrorFor(x => x.Password)
                .WithErrorCode(ValidationErrorCodes.Password.RequireNonAlphanumeric);
        }

        [Fact]
        public void PasswordRequireLength()
        {
            var req = new RegisterCommand {Password = "no"};

            var validationResult = _validator.TestValidate(req);

            validationResult
                .ShouldHaveValidationErrorFor(x => x.Password)
                .WithErrorCode(ValidationErrorCodes.Password.RequireLength);
        }
    }
}