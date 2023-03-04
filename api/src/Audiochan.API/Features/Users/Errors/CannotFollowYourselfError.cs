using System;
using Audiochan.Shared.Models;

namespace Audiochan.API.Features.Users.Errors;

public class CannotFollowYourselfError : IUserError
{
    public CannotFollowYourselfError(CannotFollowYourselfException _)
    {
        Code = GetType().Name;
        Message = "Cannot follow yourself. Silly.";
    }

    public string Code { get; }
    public string Message { get; }
}

public class CannotFollowYourselfException : Exception
{
    public CannotFollowYourselfException()
    {
        
    }
}