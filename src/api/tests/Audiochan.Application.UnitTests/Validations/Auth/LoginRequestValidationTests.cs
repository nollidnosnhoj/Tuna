using Audiochan.Application.Features.Auth.Commands.Login;
using FluentValidation;
using FluentValidation.TestHelper;
using Xunit;

namespace Audiochan.Application.UnitTests.Validations.Auth
{
    public class LoginRequestValidationTests
    {
        private readonly IValidator<LoginCommand> _validator;

        public LoginRequestValidationTests()
        {
            _validator = new LoginCommandValidator();
        }

        [Fact]
        public void CheckIfUsernameIsEmpty()
        {
            var result = _validator.TestValidate(new LoginCommand("", "testpassword123"));
            result.ShouldHaveValidationErrorFor(x => x.Login);
        }

        [Fact]
        public void CheckIfPasswordIsEmpty()
        {
            var result = _validator.TestValidate(new LoginCommand("testuser", ""));
            result.ShouldHaveValidationErrorFor(x => x.Password);
        }
    }
}