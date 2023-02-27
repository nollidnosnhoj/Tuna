﻿namespace Audiochan.Common.Models;

public interface IUserError
{
    public string Code { get; }
    public string Message { get; }
}

public record UserError(string Message, string Code) : IUserError;