using Audiochan.Core.Users.Commands;
using FluentAssertions;
using FluentValidation;
using FluentValidation.TestHelper;
using Xunit;

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
        [InlineData("applesauce@example.com")]
        [InlineData("example@example.com")]
        [InlineData("testuser123@example.com")]
        public void ShouldBeValid(string email)
        {
            var request = new UpdateEmailCommand {NewEmail = email};
            var testResults = _validator.TestValidate(request);
            testResults.IsValid.Should().BeTrue();
        }
        
        [Fact]
        public void ShouldNotBeValid_WhenEmailIsMissing()
        {
            var request = new UpdateEmailCommand {NewEmail = ""};
            var testResults = _validator.TestValidate(request);
            testResults.ShouldHaveValidationErrorFor(x => x.NewEmail);
        }
        
        [Theory]
        [InlineData("testuser123")]
        [InlineData("example123")]
        public void ShouldNotBeValid_WhenEmailIsInvalid(string email)
        {
            var request = new UpdateEmailCommand {NewEmail = email};
            var testResults = _validator.TestValidate(request);
            testResults.ShouldHaveValidationErrorFor(x => x.NewEmail);
        }
    }
}