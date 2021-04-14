using System.Collections.Generic;
using System.IO;
using System.Linq;
using Audiochan.Core.Constants;
using Audiochan.Core.Settings;
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
                .Must(fileSize => fileSize > fileSizeLimit)
                .WithMessage($"File size is over {fileSizeLimit / 1000000} MB");
        }

        public static IRuleBuilder<T, string> Password<T>(this IRuleBuilder<T, string> ruleBuilder,
            IdentitySettings identitySettings, string field = "Password")
        {
            if (identitySettings.PasswordRequiresDigit)
                ruleBuilder.Matches(@"[0-9]+")
                    .WithErrorCode(ValidationErrorCodes.Password.RequireDigits)
                    .WithMessage($"{field} must contain one digit.");
            if (identitySettings.PasswordRequiresLowercase)
                ruleBuilder.Matches(@"[a-z]+")
                    .WithErrorCode(ValidationErrorCodes.Password.RequireLowercase)
                    .WithMessage($"{field} must contain one lowercase character.");
            if (identitySettings.PasswordRequiresUppercase)
                ruleBuilder.Matches(@"[A-Z]+")
                    .WithErrorCode(ValidationErrorCodes.Password.RequireUppercase)
                    .WithMessage($"{field} must contain one uppercase character.");
            if (identitySettings.PasswordRequiresNonAlphanumeric)
                ruleBuilder.Matches(@"[^a-zA-Z\d]+")
                    .WithErrorCode(ValidationErrorCodes.Password.RequireNonAlphanumeric)
                    .WithMessage($"{field} must contain one non-alphanumeric character.");
            if (identitySettings.PasswordMinimumLength > 0)
                ruleBuilder.MinimumLength(identitySettings.PasswordMinimumLength)
                    .WithErrorCode(ValidationErrorCodes.Password.RequireLength)
                    .WithMessage($"{field} must be at least {identitySettings.PasswordMinimumLength} characters long.");

            return ruleBuilder;
        }

        public static IRuleBuilder<T, string> Username<T>(this IRuleBuilder<T, string> ruleBuilder,
            IdentitySettings identitySettings)
        {
            return ruleBuilder
                .NotEmpty()
                .WithMessage("Username is required.")
                .MinimumLength(identitySettings.UsernameMinimumLength)
                .WithMessage("Username must be at least 3 characters long.")
                .MaximumLength(identitySettings.UsernameMaximumLength)
                .WithMessage("Username must be at most 20 characters long.")
                .Must(username => username.All(x => identitySettings.UsernameAllowedCharacters.Contains(x)))
                .WithErrorCode(ValidationErrorCodes.Username.RequireCharacters)
                .WithMessage("Username is invalid.")
                .Must(username => !username.All(char.IsDigit))
                .WithErrorCode(ValidationErrorCodes.Username.InvalidFormat)
                .WithMessage("Username cannot contain only numbers.");
        }
    }
}