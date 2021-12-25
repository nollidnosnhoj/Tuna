using System.Collections.Generic;
using System.Linq;

namespace Audiochan.Application.Commons.Exceptions;

public class ValidationException : BadRequestException
{
    public IDictionary<string, string[]> ValidationErrors { get; }

    public ValidationException(Dictionary<string, string[]> errors) : base("Invalid request.")
    {
        ValidationErrors = errors;
    }

    public ValidationException(FluentValidation.ValidationException validationException) : base("Invalid request.")
    {
        ValidationErrors = validationException.Errors
            .GroupBy(x => x.PropertyName, x => x.ErrorMessage)
            .ToDictionary(x => x.Key, x => x.ToArray());
    }
}