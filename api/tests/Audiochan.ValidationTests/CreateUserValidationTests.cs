using Audiochan.Core.Common.Constants;
using Audiochan.Core.Common.Models;
using Audiochan.Core.Features.Auth.Models;
using Audiochan.Core.Features.Auth.Validators;
using FluentValidation.TestHelper;
using Xunit;

namespace Audiochan.ValidationTests
{
    public class CreateUserValidationTests
    {
        private readonly CreateUserRequestValidator _validator;

        public CreateUserValidationTests()
        {
            _validator = new CreateUserRequestValidator(new PasswordSetting
            {
                RequireDigit = true,
                RequireLowercase = true,
                RequireUppercase = true,
                RequireNonAlphanumeric = true,
                RequireLength = 5
            });
        }

        [Fact]
        public void UsernameRequired()
        {
            var req = new CreateUserRequest();
            _validator.TestValidate(req)
                .ShouldHaveValidationErrorFor(x => x.Username);
        }

        [Fact]
        public void UsernameRequiredCharacters()
        {
            var req = new CreateUserRequest {Username = "@pplesAnd&&&&"};
            _validator.TestValidate(req)
                .ShouldHaveValidationErrorFor(x => x.Username)
                .WithErrorCode(ValidationErrorCodes.RequireCharacters);
        }

        [Fact]
        public void PasswordRequired()
        {
            var req = new CreateUserRequest();
            var validationResult = _validator.TestValidate(req);
            validationResult
                .ShouldHaveValidationErrorFor(x => x.Password);
        }

        [Fact]
        public void PasswordRequireDigits()
        {
            var req = new CreateUserRequest {Password = "thisdoesnothavedigits"};

            var validationResult = _validator.TestValidate(req);

            validationResult
                .ShouldHaveValidationErrorFor(x => x.Password)
                .WithErrorCode(ValidationErrorCodes.RequireDigits);
        }

        [Fact]
        public void PasswordRequireLowercase()
        {
            var req = new CreateUserRequest {Password = "OMEGALUL4HEAD"};

            var validationResult = _validator.TestValidate(req);

            validationResult
                .ShouldHaveValidationErrorFor(x => x.Password)
                .WithErrorCode(ValidationErrorCodes.RequireLowercase);
        }
        
        [Fact]
        public void PasswordRequireUppercase()
        {
            var req = new CreateUserRequest {Password = "omegalul4head"};

            var validationResult = _validator.TestValidate(req);

            validationResult
                .ShouldHaveValidationErrorFor(x => x.Password)
                .WithErrorCode(ValidationErrorCodes.RequireUppercase);
        }
        
        [Fact]
        public void PasswordRequireNonAlphanumeric()
        {
            var req = new CreateUserRequest {Password = "lkdsfhlksdjflksdjflks"};

            var validationResult = _validator.TestValidate(req);

            validationResult
                .ShouldHaveValidationErrorFor(x => x.Password)
                .WithErrorCode(ValidationErrorCodes.RequireNonAlphanumeric);
        }
        
        [Fact]
        public void PasswordRequireLength()
        {
            var req = new CreateUserRequest {Password = "no"};

            var validationResult = _validator.TestValidate(req);

            validationResult
                .ShouldHaveValidationErrorFor(x => x.Password)
                .WithErrorCode(ValidationErrorCodes.RequireLength);
        }
    }
}