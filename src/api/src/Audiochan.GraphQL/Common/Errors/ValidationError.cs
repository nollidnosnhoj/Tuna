using FluentValidation;
using FluentValidation.Results;

namespace Audiochan.GraphQL.Common.Errors;

public record ValidationPropertyError(string Property, string Message);

public class ValidationError : GraphQlError
{
    public IReadOnlyCollection<ValidationPropertyError> Failures { get; }
    public ValidationError(ValidationException ex) : base("Validation errors occurred.")
    {
        Failures = ex.Errors
            .Select(e => new ValidationPropertyError(e.PropertyName, e.ErrorMessage))
            .ToList();
    }
}