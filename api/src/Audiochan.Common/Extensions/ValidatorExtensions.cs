using FluentValidation;

namespace Audiochan.Common.Extensions
{
    
    public class UsernameRules
    {
        public int MinimumLength { get; init; } = 3;
        public int MaximumLength { get; init; } = 20;
        public string AllowedCharacters { get; init; } = "abcdefghijklmnopqrstuvwxyz0123456789-_";
    }

    public class PasswordRules
    {
        public bool RequiresUppercase { get; init; } = true;
        public bool RequiresLowercase { get; init; } = true;
        public bool RequiresNonAlphanumeric { get; init; } = true;
        public bool RequiresDigit { get; init; } = true;
        public int MinimumLength { get; init; } = 6;
    }
    
    public static class ValidatorExtensions
    {
        public static IRuleBuilder<T, string> FileNameValidation<T>(this IRuleBuilder<T, string> ruleBuilder,
            IEnumerable<string> validContentTypes)
        {
            return ruleBuilder
                .NotEmpty()
                .WithMessage("Filename cannot be empty.")
                .Must(fileName =>
                {
                    var isContentType = fileName.TryGetContentType(out var contentType);
                    return isContentType && validContentTypes.Contains(contentType);
                })
                .WithMessage("File name is invalid.");
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

        public static IRuleBuilder<T, string> PasswordValidation<T>(this IRuleBuilder<T, string> ruleBuilder,
            PasswordRules passwordRules, string field = "Password")
        {
            if (passwordRules.RequiresDigit)
                ruleBuilder.Matches(@"[0-9]+")
                    .WithErrorCode(ValidationErrorCodes.Password.DIGITS)
                    .WithMessage($"{field} must contain one digit.");
            if (passwordRules.RequiresLowercase)
                ruleBuilder.Matches(@"[a-z]+")
                    .WithErrorCode(ValidationErrorCodes.Password.LOWERCASE)
                    .WithMessage($"{field} must contain one lowercase character.");
            if (passwordRules.RequiresUppercase)
                ruleBuilder.Matches(@"[A-Z]+")
                    .WithErrorCode(ValidationErrorCodes.Password.UPPERCASE)
                    .WithMessage($"{field} must contain one uppercase character.");
            if (passwordRules.RequiresNonAlphanumeric)
                ruleBuilder.Matches(@"[^a-zA-Z\d]+")
                    .WithErrorCode(ValidationErrorCodes.Password.NON_ALPHANUMERIC)
                    .WithMessage($"{field} must contain one non-alphanumeric character.");
            if (passwordRules.MinimumLength > 0)
                ruleBuilder.MinimumLength(passwordRules.MinimumLength)
                    .WithErrorCode(ValidationErrorCodes.Password.LENGTH)
                    .WithMessage($"{field} must be at least {passwordRules.MinimumLength} characters long.");

            return ruleBuilder;
        }

        public static IRuleBuilder<T, string> UsernameValidation<T>(this IRuleBuilder<T, string> ruleBuilder,
            UsernameRules identitySettings)
        {
            return ruleBuilder
                .NotEmpty()
                .WithMessage("Username is required.")
                .MinimumLength(identitySettings.MinimumLength)
                .WithMessage("Username must be at least 3 characters long.")
                .MaximumLength(identitySettings.MaximumLength)
                .WithMessage("Username must be at most 20 characters long.")
                .Must(username => username.All(x => identitySettings.AllowedCharacters.Contains(x)))
                .WithErrorCode(ValidationErrorCodes.Username.CHARACTERS)
                .WithMessage("Username is invalid.")
                .Must(username => !username.All(char.IsDigit))
                .WithErrorCode(ValidationErrorCodes.Username.FORMAT)
                .WithMessage("Username cannot contain only numbers.");
        }
    }
}