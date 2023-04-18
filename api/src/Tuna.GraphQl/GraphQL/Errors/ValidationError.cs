using System.Collections.Generic;
using System.Linq;
using FluentValidation;
using HotChocolate.Types;
using Tuna.Shared.Models;

namespace Tuna.GraphQl.GraphQL.Errors;

public record FieldError(string Field, string Message);

public class ValidationError : IUserError
{
    public ValidationError(IEnumerable<FieldError> errors)
    {
        FieldErrors = errors.ToList();
    }

    public IReadOnlyCollection<FieldError> FieldErrors { get; }

    public string Code => GetType().Name;
    public string Message => "Validation errors has occurred.";

    public static ValidationError CreateErrorFrom(ValidationException exception)
    {
        return new ValidationError(
            exception.Errors.Select(x => new FieldError(x.PropertyName, x.ErrorMessage)));
    }
}

public class UseValidationError : ErrorAttribute
{
    public UseValidationError() : base(typeof(ValidationError))
    {
    }
}