using System;

namespace Audiochan.Core.Features.Users.Exceptions;

public class PasswordDoesNotMatchException : Exception
{
    public PasswordDoesNotMatchException()
        : base("Passwords does not match.")
    {
        
    }
}