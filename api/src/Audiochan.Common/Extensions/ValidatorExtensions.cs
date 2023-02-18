using FluentValidation;
using FluentValidation.Results;

namespace Audiochan.Common.Extensions
{
    public static class ValidatorExtensions
    {
        public static void ThrowIfValidationFailed(this ValidationResult result)
        {
            if (!result.IsValid)
            {
                throw new ValidationException(result.Errors);
            }
        }
        
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

        public static IRuleBuilder<T, string> PasswordValidation<T>(this IRuleBuilder<T, string> ruleBuilder, string field = "Password")
        {
            ruleBuilder.Matches(@"[0-9]+")
                    .WithErrorCode(ValidationErrorCodes.Password.DIGITS)
                    .WithMessage($"{field} must contain one digit.")
                    .Matches(@"[a-z]+")
                    .WithErrorCode(ValidationErrorCodes.Password.LOWERCASE)
                    .WithMessage($"{field} must contain one lowercase character.")
                    .Matches(@"[A-Z]+")
                    .WithErrorCode(ValidationErrorCodes.Password.UPPERCASE)
                    .WithMessage($"{field} must contain one uppercase character.")
                    .Matches(@"[^a-zA-Z\d]+")
                    .WithErrorCode(ValidationErrorCodes.Password.NON_ALPHANUMERIC)
                    .WithMessage($"{field} must contain one non-alphanumeric character.")
                    .MinimumLength(6)
                    .WithErrorCode(ValidationErrorCodes.Password.LENGTH)
                    .WithMessage($"{field} must be at least 6 characters long.");

            return ruleBuilder;
        }

        public static IRuleBuilder<T, string> UsernameValidation<T>(this IRuleBuilder<T, string> ruleBuilder)
        {
            return ruleBuilder
                .NotEmpty()
                .WithMessage("Username is required.")
                .MinimumLength(3)
                .WithMessage("Username must be at least 3 characters long.")
                .MaximumLength(20)
                .WithMessage("Username must be at most 20 characters long.")
                .Must(username => username.All(x => "abcdefghijklmnopqrstuvwxyz0123456789-_".Contains(x)))
                .WithErrorCode(ValidationErrorCodes.Username.CHARACTERS)
                .WithMessage("Username is invalid.")
                .Must(username => !username.All(char.IsDigit))
                .WithErrorCode(ValidationErrorCodes.Username.FORMAT)
                .WithMessage("Username cannot contain only numbers.");
        }
    }
}