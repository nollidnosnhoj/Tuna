using Audiochan.Core.Features.Auth.Models;
using Audiochan.Core.Features.Auth.Validators;
using FluentValidation;
using FluentValidation.TestHelper;
using Xunit;

namespace Audiochan.UnitTests.Validations
{
    public class LoginValidationTests
    {
        private readonly IValidator<LoginRequest> _validator;

        public LoginValidationTests()
        {
            _validator = new LoginRequestRequestValidator();
        }

        [Fact]
        public void CheckIfUsernameIsEmpty()
        {
            var result = _validator.TestValidate(new LoginRequest());
            result.ShouldHaveValidationErrorFor(x => x.Username);
        }
        
        [Fact]
        public void CheckIfPasswordIsEmpty()
        {
            var result = _validator.TestValidate(new LoginRequest());
            result.ShouldHaveValidationErrorFor(x => x.Password);
        }
    }
}