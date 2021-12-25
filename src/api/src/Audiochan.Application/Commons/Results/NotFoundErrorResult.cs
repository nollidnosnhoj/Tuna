using System;

namespace Audiochan.Application.Commons.Results;

/// <summary>
/// Represents a failed result because a resource was not found.
/// </summary>
public class NotFoundErrorResult : ErrorResult
{
    public NotFoundErrorResult() : base("Resource was not found.")
    {
    }

    public NotFoundErrorResult(string message) : base(message)
    {
    }

    public NotFoundErrorResult(Type type) : base($"{type.Name} was not found.")
    {
    }
}

/// <summary>
/// Represents a failed result because a resource was not found.
/// </summary>
public class NotFoundErrorResult<T> : ErrorResult<T>
{
    public NotFoundErrorResult() : base("Resource was not found.")
    {
    }
    
    public NotFoundErrorResult(string message) : base(message)
    {
    }
    
    public NotFoundErrorResult(Type type) : base($"{type.Name} was not found.")
    {
    }
}