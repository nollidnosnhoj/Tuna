using FluentValidation;
using FluentValidation.Results;

namespace Tuna.Shared.Extensions;

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
}