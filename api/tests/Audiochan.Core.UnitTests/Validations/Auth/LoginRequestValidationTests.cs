using Audiochan.Core.Features.Auth.Login;
using FluentValidation;
using FluentValidation.TestHelper;
using Xunit;

namespace Audiochan.Core.UnitTests.Validations.Auth
{
    public class LoginRequestValidationTests
    {
        private readonly IValidator<LoginRequest> _validator;

        public LoginRequestValidationTests()
        {
            _validator = new LoginRequestValidator();
        }

        [Fact]
        public void CheckIfUsernameIsEmpty()
        {
            var result = _validator.TestValidate(new LoginRequest());
            result.ShouldHaveValidationErrorFor(x => x.Login);
        }

        [Fact]
        public void CheckIfPasswordIsEmpty()
        {
            var result = _validator.TestValidate(new LoginRequest());
            result.ShouldHaveValidationErrorFor(x => x.Password);
        }
    }
}