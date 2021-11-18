using Audiochan.Core.Auth;
using Audiochan.Core.Auth.Commands;
using FluentValidation;
using FluentValidation.TestHelper;
using NUnit.Framework;

namespace Audiochan.Core.UnitTests.Validations.Auth
{
    public class LoginRequestValidationTests
    {
        private readonly IValidator<LoginCommand> _validator;

        public LoginRequestValidationTests()
        {
            _validator = new LoginCommandValidator();
        }

        [Test]
        public void CheckIfUsernameIsEmpty()
        {
            var result = _validator.TestValidate(new LoginCommand());
            result.ShouldHaveValidationErrorFor(x => x.Login);
        }

        [Test]
        public void CheckIfPasswordIsEmpty()
        {
            var result = _validator.TestValidate(new LoginCommand());
            result.ShouldHaveValidationErrorFor(x => x.Password);
        }
    }
}