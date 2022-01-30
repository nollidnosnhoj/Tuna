using Audiochan.Core.Features.Users.Commands.UpdateUsername;
using Bogus;
using FluentAssertions;
using FluentValidation;
using FluentValidation.TestHelper;
using Microsoft.Extensions.Options;
using Xunit;

namespace Audiochan.Core.UnitTests.Validations.Users
{
    public class UpdateUsernameCommandValidatorTests
    {
        private readonly IValidator<UpdateUsernameCommand> _validator;

        public UpdateUsernameCommandValidatorTests()
        {
            var options = Options.Create(new IdentitySettings());
            _validator = new UpdateUsernameCommandValidator(options);
        }

        [Theory]
        [InlineData("applesauce")]
        [InlineData("testuser123")]
        public void ShouldBeValid(string username)
        {
            var request = new UpdateUsernameCommand {NewUsername = username};
            var testResults = _validator.TestValidate(request);
            testResults.IsValid.Should().BeTrue();
        }
        
        [Fact]
        public void ShouldNotBeValid_WhenUsernameIsMissing()
        {
            var request = new UpdateUsernameCommand{NewUsername = ""};
            var testResults = _validator.TestValidate(request);
            testResults.ShouldHaveValidationErrorFor(x => x.NewUsername);
        }
        
        [Theory]
        [InlineData("a")]
        [InlineData("ab")]
        public void ShouldNotBeValid_WhenUsernameIsTooShort(string username)
        {
            var request = new UpdateUsernameCommand {NewUsername = username};
            var testResults = _validator.TestValidate(request);
            testResults.ShouldHaveValidationErrorFor(x => x.NewUsername);
        }
        
        [Fact]
        public void ShouldNotBeValid_WhenUsernameIsTooLong()
        {
            var faker = new Faker();
            var username = faker.Random.String2(21);
            var request = new UpdateUsernameCommand {NewUsername = username};
            var testResults = _validator.TestValidate(request);
            testResults.ShouldHaveValidationErrorFor(x => x.NewUsername);
        }
        
        [Theory]
        [InlineData("username!")]
        [InlineData("username@example.com")]
        public void ShouldNotBeValid_WhenUsernameContainsIllegalCharacters(string username)
        {
            var request = new UpdateUsernameCommand {NewUsername = username};
            var testResults = _validator.TestValidate(request);
            testResults.ShouldHaveValidationErrorFor(x => x.NewUsername);
        }
        
        [Theory]
        [InlineData("12345")]
        [InlineData("0000000000000")]
        public void ShouldNotBeValid_WhenUsernameOnlyContainDigits(string username)
        {
            var request = new UpdateUsernameCommand {NewUsername = username};
            var testResults = _validator.TestValidate(request);
            testResults.ShouldHaveValidationErrorFor(x => x.NewUsername);
        }
    }
}