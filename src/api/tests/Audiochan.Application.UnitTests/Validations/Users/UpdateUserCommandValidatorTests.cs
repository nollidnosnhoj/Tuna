using Audiochan.Application.Features.Users.Commands.UpdateUser;
using Bogus;
using FluentAssertions;
using FluentValidation;
using FluentValidation.TestHelper;
using Microsoft.Extensions.Options;
using Xunit;

namespace Audiochan.Application.UnitTests.Validations.Users
{
    public class UpdateUserCommandValidatorTests
    {
        private readonly IValidator<UpdateUserCommand> _validator;

        public UpdateUserCommandValidatorTests()
        {
            var options = Options.Create(new IdentitySettings());
            _validator = new UpdateUserCommandValidator(options);
        }

        [Theory]
        [InlineData("applesauce@example.com")]
        [InlineData("example@example.com")]
        [InlineData("testuser123@example.com")]
        public void ShouldBeValid_WhenEmailIsValid(string email)
        {
            var request = new UpdateUserCommand(1, "testuser", email);
            var testResults = _validator.TestValidate(request);
            testResults.IsValid.Should().BeTrue();
        }
        
        [Fact]
        public void ShouldNotBeValid_WhenEmailIsMissing()
        {
            var request = new UpdateUserCommand(1, "testuser", "");
            var testResults = _validator.TestValidate(request);
            testResults.ShouldHaveValidationErrorFor(x => x.Email);
        }
        
        [Theory]
        [InlineData("testuser123")]
        [InlineData("example123")]
        public void ShouldNotBeValid_WhenEmailIsInvalid(string email)
        {
            var request = new UpdateUserCommand(1, "testuser", email);
            var testResults = _validator.TestValidate(request);
            testResults.ShouldHaveValidationErrorFor(x => x.Email);
        }
        
        [Theory]
        [InlineData("applesauce")]
        [InlineData("testuser123")]
        public void ShouldBeValid_WhenUsernameIsValid(string username)
        {
            var request = new UpdateUserCommand(1, username, "testuser@example.com");
            var testResults = _validator.TestValidate(request);
            testResults.IsValid.Should().BeTrue();
        }
        
        [Fact]
        public void ShouldNotBeValid_WhenUsernameIsMissing()
        {
            var request = new UpdateUserCommand(1, "", "testuser@example.com");
            var testResults = _validator.TestValidate(request);
            testResults.ShouldHaveValidationErrorFor(x => x.Username);
        }
        
        [Theory]
        [InlineData("a")]
        [InlineData("ab")]
        public void ShouldNotBeValid_WhenUsernameIsTooShort(string username)
        {
            var request = new UpdateUserCommand(1, username, "testuser@example.com");
            var testResults = _validator.TestValidate(request);
            testResults.ShouldHaveValidationErrorFor(x => x.Username);
        }
        
        [Fact]
        public void ShouldNotBeValid_WhenUsernameIsTooLong()
        {
            var faker = new Faker();
            var username = faker.Random.String2(21);
            var request = new UpdateUserCommand(1, username, "testuser@example.com");
            var testResults = _validator.TestValidate(request);
            testResults.ShouldHaveValidationErrorFor(x => x.Username);
        }
        
        [Theory]
        [InlineData("username!")]
        [InlineData("username@example.com")]
        public void ShouldNotBeValid_WhenUsernameContainsIllegalCharacters(string username)
        {
            var request = new UpdateUserCommand(1, username, "testuser@example.com");
            var testResults = _validator.TestValidate(request);
            testResults.ShouldHaveValidationErrorFor(x => x.Username);
        }
        
        [Theory]
        [InlineData("12345")]
        [InlineData("0000000000000")]
        public void ShouldNotBeValid_WhenUsernameOnlyContainDigits(string username)
        {
            var request = new UpdateUserCommand(1, username, "testuser@example.com");
            var testResults = _validator.TestValidate(request);
            testResults.ShouldHaveValidationErrorFor(x => x.Username);
        }
    }
}