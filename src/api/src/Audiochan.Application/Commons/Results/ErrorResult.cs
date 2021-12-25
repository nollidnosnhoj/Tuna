using System;
using System.Collections.Generic;
using Audiochan.Application.Commons.Interfaces;

namespace Audiochan.Application.Commons.Results;



public record Error(string Code, string Message);

/// <summary>
/// Represents a failed result with errors
/// </summary>
public class ErrorResult : Result, IErrorResult
{
    public string Message { get; }
    public IReadOnlyCollection<Error> Errors { get; }

    public ErrorResult(string message, IReadOnlyCollection<Error>? errors = null)
    {
        Message = message;
        Errors = errors ?? Array.Empty<Error>();
    }
}

/// <summary>
/// Represents a failed result with errors
/// </summary>
public class ErrorResult<T> : Result<T>, IErrorResult
{
    public string Message { get; }
    public IReadOnlyCollection<Error> Errors { get; }

    public ErrorResult(string message, IReadOnlyCollection<Error>? errors = null) : base(default!)
    {
        Message = message;
        Errors = errors ?? Array.Empty<Error>();
    }

    public static implicit operator ErrorResult(ErrorResult<T> errorResult) => new(errorResult.Message);
}