using Audiochan.Core.Features.Auth.Commands.Login;
using FluentValidation;
using FluentValidation.TestHelper;
using Xunit;

namespace Audiochan.Core.UnitTests.Validations.Auth
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
            var result = _validator.TestValidate(new LoginCommand());
            result.ShouldHaveValidationErrorFor(x => x.Login);
        }

        [Fact]
        public void CheckIfPasswordIsEmpty()
        {
            var result = _validator.TestValidate(new LoginCommand());
            result.ShouldHaveValidationErrorFor(x => x.Password);
        }
    }
}