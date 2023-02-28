using System;
using Audiochan.Common.Models;

namespace Audiochan.API.Features.Users.Errors;

public class CannotFollowYourselfError : IUserError
{
    public string Code => GetType().Name;
    public string Message => "Cannot follow yourself. Silly.";
}

public class CannotFollowYourselfException : Exception
{
    public CannotFollowYourselfException()
    {
        
    }
}