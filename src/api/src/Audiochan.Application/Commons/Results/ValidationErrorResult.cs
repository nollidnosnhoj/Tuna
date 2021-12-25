using System;
using System.Collections.Generic;

namespace Audiochan.Application.Commons.Results;

public record ValidationError(string Property, string Message);

/// <summary>
/// Represents a failed result because of invalid parameters
/// </summary>
public class ValidationErrorResult : ErrorResult
{
    public IReadOnlyCollection<ValidationError> ValidationErrors { get; init; } = Array.Empty<ValidationError>();

    public ValidationErrorResult(string message) : base(message)
    {
    }

    public ValidationErrorResult(string message, IReadOnlyCollection<ValidationError> errors) 
        : base(message)
    {
        ValidationErrors = errors;
    }
}

/// <summary>
/// Represents a failed result because of invalid parameters
/// </summary>
public class ValidationErrorResult<T> : ErrorResult<T>
{
    public IReadOnlyCollection<ValidationError> ValidationErrors { get; init; } = Array.Empty<ValidationError>();

    public ValidationErrorResult(string message) : base(message)
    {
    }

    public ValidationErrorResult(string message, IReadOnlyCollection<ValidationError> errors) 
        : base(message)
    {
        ValidationErrors = errors;
    }
}