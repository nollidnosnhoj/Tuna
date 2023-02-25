using System;

namespace Audiochan.Core.Features.Users.Exceptions;

public class CannotFollowYourselfException : Exception
{
    public CannotFollowYourselfException()
        : base("Cannot follow yourself.")
    {
        
    }
}