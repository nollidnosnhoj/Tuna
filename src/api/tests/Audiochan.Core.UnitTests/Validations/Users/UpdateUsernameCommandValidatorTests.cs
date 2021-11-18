using Audiochan.Core.Common;
using Audiochan.Core.Users;
using Audiochan.Core.Users.Commands;
using Bogus;
using FluentAssertions;
using FluentValidation;
using FluentValidation.TestHelper;
using Microsoft.Extensions.Options;
using NUnit.Framework;

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
        [TestCase("applesauce")]
        [TestCase("testuser123")]
        public void ShouldBeValid(string username)
        {
            var request = new UpdateUsernameCommand {NewUsername = username};
            var testResults = _validator.TestValidate(request);
            testResults.IsValid.Should().BeTrue();
        }
        
        [Test]
        public void ShouldNotBeValid_WhenUsernameIsMissing()
        {
            var request = new UpdateUsernameCommand{NewUsername = ""};
            var testResults = _validator.TestValidate(request);
            testResults.ShouldHaveValidationErrorFor(x => x.NewUsername);
        }
        
        [Theory]
        [TestCase("a")]
        [TestCase("ab")]
        public void ShouldNotBeValid_WhenUsernameIsTooShort(string username)
        {
            var request = new UpdateUsernameCommand {NewUsername = username};
            var testResults = _validator.TestValidate(request);
            testResults.ShouldHaveValidationErrorFor(x => x.NewUsername);
        }
        
        [Test]
        public void ShouldNotBeValid_WhenUsernameIsTooLong()
        {
            var faker = new Faker();
            var username = faker.Random.String2(21);
            var request = new UpdateUsernameCommand {NewUsername = username};
            var testResults = _validator.TestValidate(request);
            testResults.ShouldHaveValidationErrorFor(x => x.NewUsername);
        }
        
        [Theory]
        [TestCase("username!")]
        [TestCase("username@example.com")]
        public void ShouldNotBeValid_WhenUsernameContainsIllegalCharacters(string username)
        {
            var request = new UpdateUsernameCommand {NewUsername = username};
            var testResults = _validator.TestValidate(request);
            testResults.ShouldHaveValidationErrorFor(x => x.NewUsername);
        }
        
        [Theory]
        [TestCase("12345")]
        [TestCase("0000000000000")]
        public void ShouldNotBeValid_WhenUsernameOnlyContainDigits(string username)
        {
            var request = new UpdateUsernameCommand {NewUsername = username};
            var testResults = _validator.TestValidate(request);
            testResults.ShouldHaveValidationErrorFor(x => x.NewUsername);
        }
    }
}