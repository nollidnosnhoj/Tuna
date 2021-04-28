using System.Collections.Generic;
using System.IO;
using System.Linq;
using Audiochan.Core.Constants;
using Audiochan.Core.Models.Settings;
using FluentValidation;

namespace Audiochan.Core.Extensions
{
    public static class ValidatorExtensions
    {
        public static IRuleBuilder<T, string> FileNameValidation<T>(this IRuleBuilder<T, string> ruleBuilder, 
            IEnumerable<string> validContentTypes)
        {
            return ruleBuilder
                .NotEmpty()
                .WithMessage("Filename cannot be empty.")
                .Must(Path.HasExtension)
                .WithMessage("Filename must have a file extension")
                .Must(value => validContentTypes.Contains(value.GetContentType()))
                .WithMessage("The file name's extension is invalid.");
        }

        public static IRuleBuilder<T, string> FileContentTypeValidation<T>(this IRuleBuilder<T, string> ruleBuilder,
            IEnumerable<string> contentTypes)
        {
            return ruleBuilder
                .NotEmpty()
                .WithMessage("File's content type cannot be empty.")
                .Must(contentTypes.Contains)
                .WithMessage("File's content type is invalid.");
        }

        public static IRuleBuilder<T, long> FileSizeValidation<T>(this IRuleBuilder<T, long> ruleBuilder,
            long fileSizeLimit)
        {
            return ruleBuilder
                .NotEmpty()
                .WithMessage("Filesize cannot be empty.")
                .LessThanOrEqualTo(fileSizeLimit)
                .WithMessage($"File size is over {fileSizeLimit / 1000000} MB");
        }

        public static IRuleBuilder<T, string> Password<T>(this IRuleBuilder<T, string> ruleBuilder,
            IdentitySettings.PasswordRules passwordRules, string field = "Password")
        {
            if (passwordRules.RequiresDigit)
                ruleBuilder.Matches(@"[0-9]+")
                    .WithErrorCode(ValidationErrorCodes.Password.RequireDigits)
                    .WithMessage($"{field} must contain one digit.");
            if (passwordRules.RequiresLowercase)
                ruleBuilder.Matches(@"[a-z]+")
                    .WithErrorCode(ValidationErrorCodes.Password.RequireLowercase)
                    .WithMessage($"{field} must contain one lowercase character.");
            if (passwordRules.RequiresUppercase)
                ruleBuilder.Matches(@"[A-Z]+")
                    .WithErrorCode(ValidationErrorCodes.Password.RequireUppercase)
                    .WithMessage($"{field} must contain one uppercase character.");
            if (passwordRules.RequiresNonAlphanumeric)
                ruleBuilder.Matches(@"[^a-zA-Z\d]+")
                    .WithErrorCode(ValidationErrorCodes.Password.RequireNonAlphanumeric)
                    .WithMessage($"{field} must contain one non-alphanumeric character.");
            if (passwordRules.MinimumLength > 0)
                ruleBuilder.MinimumLength(passwordRules.MinimumLength)
                    .WithErrorCode(ValidationErrorCodes.Password.RequireLength)
                    .WithMessage($"{field} must be at least {passwordRules.MinimumLength} characters long.");

            return ruleBuilder;
        }

        public static IRuleBuilder<T, string> Username<T>(this IRuleBuilder<T, string> ruleBuilder,
            IdentitySettings.UsernameRules identitySettings)
        {
            return ruleBuilder
                .NotEmpty()
                .WithMessage("Username is required.")
                .MinimumLength(identitySettings.MinimumLength)
                .WithMessage("Username must be at least 3 characters long.")
                .MaximumLength(identitySettings.MaximumLength)
                .WithMessage("Username must be at most 20 characters long.")
                .Must(username => username.All(x => identitySettings.AllowedCharacters.Contains(x)))
                .WithErrorCode(ValidationErrorCodes.Username.RequireCharacters)
                .WithMessage("Username is invalid.")
                .Must(username => !username.All(char.IsDigit))
                .WithErrorCode(ValidationErrorCodes.Username.InvalidFormat)
                .WithMessage("Username cannot contain only numbers.");
        }
    }
}