using Audiochan.Core.Users;
using Audiochan.Core.Users.Commands;
using FluentAssertions;
using FluentValidation;
using FluentValidation.TestHelper;
using NUnit.Framework;

namespace Audiochan.Core.UnitTests.Validations.Users
{
    public class UpdateEmailCommandValidatorTests
    {
        private readonly IValidator<UpdateEmailCommand> _validator;

        public UpdateEmailCommandValidatorTests()
        {
            _validator = new UpdateEmailCommandValidator();
        }

        [Theory]
        [TestCase("applesauce@example.com")]
        [TestCase("example@example.com")]
        [TestCase("testuser123@example.com")]
        public void ShouldBeValid(string email)
        {
            var request = new UpdateEmailCommand {NewEmail = email};
            var testResults = _validator.TestValidate(request);
            testResults.IsValid.Should().BeTrue();
        }
        
        [Test]
        public void ShouldNotBeValid_WhenEmailIsMissing()
        {
            var request = new UpdateEmailCommand {NewEmail = ""};
            var testResults = _validator.TestValidate(request);
            testResults.ShouldHaveValidationErrorFor(x => x.NewEmail);
        }
        
        [Theory]
        [TestCase("testuser123")]
        [TestCase("example123")]
        public void ShouldNotBeValid_WhenEmailIsInvalid(string email)
        {
            var request = new UpdateEmailCommand {NewEmail = email};
            var testResults = _validator.TestValidate(request);
            testResults.ShouldHaveValidationErrorFor(x => x.NewEmail);
        }
    }
}