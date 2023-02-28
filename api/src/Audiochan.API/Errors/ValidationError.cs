using System.Collections.Generic;
using System.Linq;
using Audiochan.Common.Models;
using FluentValidation;
using HotChocolate.Types;

namespace Audiochan.API.Errors;

public record FieldError(string Field, string Message);

public class ValidationError : IUserError
{
    public IReadOnlyCollection<FieldError> FieldErrors { get; }

    public ValidationError(IEnumerable<FieldError> errors)
    {
        FieldErrors = errors.ToList();
    }

    public static ValidationError CreateErrorFrom(ValidationException exception)
    {
        return new ValidationError(
            exception.Errors.Select(x => new FieldError(x.PropertyName, x.ErrorMessage)));
    }

    public string Code => GetType().Name;
    public string Message => "Validation errors has occurred.";
}

public class UseValidationError : ErrorAttribute
{
    public UseValidationError() : base(typeof(ValidationError))
    {
    }
}