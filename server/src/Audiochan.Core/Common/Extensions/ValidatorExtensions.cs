using System.Collections.Generic;
using System.Linq;
using Audiochan.Core.Common.Constants;
using Audiochan.Core.Common.Settings;
using FluentValidation;
using Microsoft.AspNetCore.Http;

namespace Audiochan.Core.Common.Extensions
{
    public static class ValidatorExtensions
    {
        /// <summary>
        /// Validate the IFormFile to ensure the file meets the given restrictions.
        /// </summary>
        /// <param name="ruleBuilder"></param>
        /// <param name="contentTypes"></param>
        /// <param name="fileSizeLimit"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static IRuleBuilder<T, IFormFile> FileValidation<T>(this IRuleBuilder<T, IFormFile> ruleBuilder,
            IEnumerable<string> contentTypes, long fileSizeLimit)
        {
            return ruleBuilder
                .Must(file => file.Length <= fileSizeLimit)
                .WithMessage($"File size is over {fileSizeLimit / 1000000} MB")
                .Must(file => contentTypes.Contains(file.ContentType))
                .WithMessage("File type is invalid.");
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
                .WithMessage("Username is invalid.");
        }
    }
}