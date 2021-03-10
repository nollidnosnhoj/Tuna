using Audiochan.Core.Features.Auth.Login;
using FluentValidation;
using FluentValidation.TestHelper;
using Xunit;

namespace Audiochan.Core.UnitTests.Validations
{
    public class LoginValidationTests
    {
        private readonly IValidator<LoginCommand> _validator;

        public LoginValidationTests()
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