using System.Collections.Generic;
using System.Linq;
using FluentValidation;

namespace Audiochan.Application.Commons.Extensions
{
    public static class ValidatorExtensions
    {
        public static IRuleBuilder<T, string> FileNameValidation<T>(this IRuleBuilder<T, string> ruleBuilder,
            IEnumerable<string> validContentTypes)
        {
            return ruleBuilder
                .NotEmpty()
                .Must(fileName =>
                {
                    var isContentType = fileName.TryGetContentType(out var contentType);
                    return isContentType && validContentTypes.Contains(contentType);
                })
                .WithMessage("The file type is not supported.");
        }

        public static IRuleBuilder<T, long> FileSizeValidation<T>(this IRuleBuilder<T, long> ruleBuilder,
            long fileSizeLimit)
        {
            return ruleBuilder
                .NotEmpty()
                .LessThanOrEqualTo(fileSizeLimit)
                .WithMessage($"The file size is over {fileSizeLimit / 1000000} MB");
        }

        public static IRuleBuilder<T, string> PasswordValidation<T>(this IRuleBuilder<T, string> ruleBuilder,
            IdentitySettings.PasswordRules passwordRules)
        {
            if (passwordRules.RequiresDigit)
                ruleBuilder.Matches(@"[0-9]+")
                    .WithErrorCode(ValidationErrorCodes.Password.DIGITS)
                    .WithMessage("The password must contain a digit.");
            if (passwordRules.RequiresLowercase)
                ruleBuilder.Matches(@"[a-z]+")
                    .WithErrorCode(ValidationErrorCodes.Password.LOWERCASE)
                    .WithMessage("The password must contain a lowercase letter.");
            if (passwordRules.RequiresUppercase)
                ruleBuilder.Matches(@"[A-Z]+")
                    .WithErrorCode(ValidationErrorCodes.Password.UPPERCASE)
                    .WithMessage("The password must contain an uppercase letter.");
            if (passwordRules.RequiresNonAlphanumeric)
                ruleBuilder.Matches(@"[^a-zA-Z\d]+")
                    .WithErrorCode(ValidationErrorCodes.Password.NON_ALPHANUMERIC)
                    .WithMessage("The password must contain a non-alphanumeric character.");
            if (passwordRules.MinimumLength > 0)
                ruleBuilder.MinimumLength(passwordRules.MinimumLength)
                    .WithErrorCode(ValidationErrorCodes.Password.LENGTH)
                    .WithMessage($"The password must be at least {passwordRules.MinimumLength} characters long.");

            return ruleBuilder;
        }

        public static IRuleBuilder<T, string> UsernameValidation<T>(this IRuleBuilder<T, string> ruleBuilder,
            IdentitySettings.UsernameRules identitySettings)
        {
            return ruleBuilder
                .NotEmpty()
                .WithMessage("The username is required.")
                .MinimumLength(identitySettings.MinimumLength)
                .WithMessage($"The username must be at least {identitySettings.MinimumLength} characters long.")
                .MaximumLength(identitySettings.MaximumLength)
                .WithMessage($"The username must be no more than {identitySettings.MaximumLength} characters long.")
                .Must(username => username.All(x => identitySettings.AllowedCharacters.Contains(x)))
                .WithErrorCode(ValidationErrorCodes.Username.CHARACTERS)
                .WithMessage("The username has invalid characters.")
                .Must(username => !username.All(char.IsDigit))
                .WithErrorCode(ValidationErrorCodes.Username.FORMAT)
                .WithMessage("The username cannot only contain digits.");
        }
    }
}