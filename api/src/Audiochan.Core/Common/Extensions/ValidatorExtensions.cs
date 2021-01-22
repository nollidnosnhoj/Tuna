using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Audiochan.Core.Common.Constants;
using Audiochan.Core.Common.Models;
using Audiochan.Core.Common.Options;
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

        public static IRuleBuilder<T, string?> Password<T>(this IRuleBuilder<T, string?> ruleBuilder, 
            IdentityUserOptions identityOptions, string field = "Password")
        {
            if (identityOptions.PasswordRequiresDigit)
                ruleBuilder.Matches(@"^[0-9]+$")
                    .WithErrorCode(ValidationErrorCodes.RequireDigits)
                    .WithMessage($"{field} must contain one digit.");
            if (identityOptions.PasswordRequiresLowercase)
                ruleBuilder.Matches(@"^[a-z]+$")
                    .WithErrorCode(ValidationErrorCodes.RequireLowercase)
                    .WithMessage($"{field} must contain one lowercase character.");
            if (identityOptions.PasswordRequiresUppercase)
                ruleBuilder.Matches(@"^[A-Z]+$")
                    .WithErrorCode(ValidationErrorCodes.RequireUppercase)
                    .WithMessage($"{field} must contain one uppercase character.");
            if (identityOptions.PasswordRequiresNonAlphanumeric)
                ruleBuilder.Matches(@"^[^a-zA-Z\d]+$")
                    .WithErrorCode(ValidationErrorCodes.RequireNonAlphanumeric)
                    .WithMessage($"{field} must contain one non-alphanumeric character.");
            if (identityOptions.PasswordMinimumLength > 0)
                ruleBuilder.MinimumLength(identityOptions.PasswordMinimumLength)
                    .WithErrorCode(ValidationErrorCodes.RequireLength)
                    .WithMessage($"{field} must be at least {identityOptions.PasswordMinimumLength} characters long.");

            return ruleBuilder;
        }

        public static IRuleBuilder<T, string?> Username<T>(this IRuleBuilder<T, string?> ruleBuilder, 
            IdentityUserOptions identityOptions)
        {
            return ruleBuilder
                .NotEmpty()
                    .WithMessage("Username is required.")
                .MinimumLength(identityOptions.UsernameMinimumLength)
                    .WithMessage("Username must be at least 3 characters long.")
                .MaximumLength(identityOptions.UsernameMaximumLength)
                    .WithMessage("Username must be at most 20 characters long.")
                .Must(username => username.All(x => identityOptions.UsernameAllowedCharacters.Contains(x)))
                    .WithErrorCode(ValidationErrorCodes.RequireCharacters)
                    .WithMessage("Username is invalid.");
        }
    }
}