using System.Collections.Generic;
using System.Linq;
using FluentValidation.Results;

namespace Audiochan.Application.Commons.Exceptions;

public class ValidationException : BadRequestException
{
    public IDictionary<string, string[]> ValidationErrors { get; }

    public ValidationException(Dictionary<string, string[]> errors) : base("Invalid request.")
    {
        ValidationErrors = errors;
    }

    public ValidationException(IEnumerable<ValidationFailure> failures) : base("Invalid request.")
    {
        ValidationErrors = failures
            .GroupBy(x => x.PropertyName, x => x.ErrorMessage)
            .ToDictionary(x => x.Key, x => x.ToArray());
    }
}