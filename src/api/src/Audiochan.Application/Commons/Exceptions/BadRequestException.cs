using System;

namespace Audiochan.Application.Commons.Exceptions;

public class BadRequestException : Exception
{
    public string[] Errors { get; }

    public BadRequestException(string message, params string[] errors) : base(message)
    {
        Errors = errors;
    }

    public BadRequestException(string message) : base(message)
    {
        Errors = Array.Empty<string>();
    }
}